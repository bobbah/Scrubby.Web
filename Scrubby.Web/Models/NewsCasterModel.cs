using System.Collections.Generic;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models;

public class NewsCasterModel
{
    public Round Round { get; set; }
    public List<NewsCasterChannel> Channels { get; set; }
    public List<NewsCasterWanted> Wanted { get; set; }
}