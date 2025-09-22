using System;
using System.Text.RegularExpressions;

namespace Scrubby.Web.Models.Data;

public class SqlCKey
{
    private static readonly Regex CKeyInvalidCharacters = new(@"[^a-z0-9]", RegexOptions.Compiled);
    public string CKey { get; set; }
    public string ByondKey { get; set; }
    public DateTime? JoinedBYOND { get; set; }
    public bool UserNotFound { get; set; }
    public bool UserInactive { get; set; }

    public static string SanitizeKey(string raw) => CKeyInvalidCharacters.Replace(raw.ToLower(), "");
}