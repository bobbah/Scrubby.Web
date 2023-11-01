using System;

namespace Scrubby.Web.Models.BYOND;

public class BYONDUserData
{
    public string Key { get; set; }
    public string CKey { get; set; }
    public DateTime Joined { get; set; }
    public bool IsMember { get; set; }
}