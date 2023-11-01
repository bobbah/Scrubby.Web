using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models;

public class RoundRuntimeModel
{
    public int RoundID { get; set; }
    public RoundBuildInfo Version { get; set; }
}