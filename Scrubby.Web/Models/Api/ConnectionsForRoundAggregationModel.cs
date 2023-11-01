using System.Collections.Generic;

namespace Scrubby.Web.Models.Api;

public class ConnectionsForRoundAggregationModel
{
    public int Round { get; set; }
    public List<string> CKeyFilter { get; set; }
}