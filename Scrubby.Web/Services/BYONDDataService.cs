using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Scrubby.Web.Models.BYOND;

namespace Scrubby.Web.Services;

// Taken from ScrubbyServer, slightly modified
public class BYONDDataService
{
    private const string BaseURL = "https://www.byond.com/";

    private static readonly Regex KVPSplit = new(@"^\s+(?<key>.+) = (?<val>.+)$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex BYONDCommandResponse = new(@"^(?<cmd>.+)\((?<response>.+)\)$", RegexOptions.Compiled);
    private readonly RestClient _client;

    public BYONDDataService()
    {
        _client = new RestClient(BaseURL);
    }

    public async Task<bool> KeyAvailable(string key, CancellationToken cancellationToken)
    {
        var request = new RestRequest(string.Empty)
            .AddQueryParameter("command", "key_available").AddQueryParameter("key", key);
        var response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Failed to successfully query against BYOND, response code {response.StatusCode}");

        var match = BYONDCommandResponse.Match(response.Content);
        if (!match.Success)
            throw new Exception(
                $"Invalid response from BYOND, unexpected response pattern. Response content: {response.Content}");

        var responseComponents = match.Groups["response"].Value.Split(',');
        return responseComponents[0] == "true";
    }

    public async Task<BYONDUserData> GetUserData(string key, CancellationToken cancellationToken)
    {
        var request =
            new RestRequest($"members/{key}").AddQueryParameter("format", "text");
        var response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK) return null;

        if (response.Content.Contains("not found.</div>"))
            throw new BYONDUserNotFoundException($"User {key} not found.");

        if (response.Content.Contains("is not active.</div>"))
            throw new BYONDUserInactiveException($"User {key} is not active.");

        var kvp = KVPSplit.Matches(response.Content);
        var dict = new Dictionary<string, string>();
        foreach (var m in kvp.AsEnumerable())
            dict.TryAdd(m.Groups["key"].Value.Trim('"'), m.Groups["val"].Value.Trim('"', '\r', '\n'));

        if (!dict.ContainsKey("joined"))
            throw new Exception("Invalid response from BYOND, no join date data found in response...");

        return new BYONDUserData
        {
            CKey = dict["ckey"],
            Key = dict["key"],
            Joined = DateTime.SpecifyKind(DateTime.Parse(dict["joined"]), DateTimeKind.Utc),
            IsMember = dict.ContainsKey("is_member") && dict["is_member"] == "1"
        };
    }
}

public class BYONDUserNotFoundException : Exception
{
    public BYONDUserNotFoundException()
    {
    }

    public BYONDUserNotFoundException(string message) : base(message)
    {
    }

    public BYONDUserNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class BYONDUserInactiveException : Exception
{
    public BYONDUserInactiveException()
    {
    }

    public BYONDUserInactiveException(string message) : base(message)
    {
    }

    public BYONDUserInactiveException(string message, Exception inner) : base(message, inner)
    {
    }
}