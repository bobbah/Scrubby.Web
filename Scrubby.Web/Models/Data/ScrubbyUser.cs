using System.Collections.Generic;

namespace Scrubby.Web.Models.Data;

public class ScrubbyUser
{
    public string PhpBBUsername { get; set; }
    public string ByondKey { get; set; }
    public string ByondCKey { get; set; }
    public List<string> Roles { get; set; }
}