using System.Collections.Generic;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models;

public class CKeyModel
{
    public List<NameCountRecord> Names { get; set; }
    public List<ServerStatistic> Playtime { get; set; }
    public CKey Key { get; set; }
    public string ByondKey { get; set; }
}