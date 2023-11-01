using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models;

public class PlayerNameStatistic
{
    public string ICName { get; set; }
    public CKey CKey { get; set; }
    public int Count { get; set; }

    public string RawCKey
    {
        get => CKey.Raw;
        set => CKey = new CKey(value);
    }
}