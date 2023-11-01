using System;
using System.Text.RegularExpressions;

namespace Scrubby.Web.Models.PostRequests;

public enum PlayerSearchType
{
    Unknown,
    CKey,
    ICName
}

public class PlayerSearchPostModel
{
    public string RegexPattern
    {
        set => Regex = new Regex(value, RegexOptions.IgnoreCase);
    }

    public Regex Regex { get; set; }

    public string SearchTypeStr
    {
        set
        {
            if (Enum.TryParse<PlayerSearchType>(value, out var newVal)) SearchType = newVal;
        }
    }

    public PlayerSearchType SearchType { get; set; }
}