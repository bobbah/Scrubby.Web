using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scrubby.Web.Models.Api;

public class DataAggregationRequestModel
{
    public int UpperRoundLimit { get; set; }
    public int LowerRoundLimit { get; set; }
    public List<string> TypeFilters { get; set; }
    public Regex ContentFilter { get; set; }
    public List<string> Files { get; set; }
    public int ResponseLimit { get; set; }
    public bool GroupByRound { get; set; }
    public bool NoMetadata { get; set; }

    public override string ToString()
    {
        return
            $"Upper: {UpperRoundLimit}, \nLower: {LowerRoundLimit}, \nType Filters: [{string.Join(", ", TypeFilters)}], \nContentFilter: {ContentFilter}, \nFiles: [{string.Join(", ", Files)}], \nResponse Limit: {ResponseLimit}, \nGroup By Round: {GroupByRound}, \nNo Metadata: {NoMetadata}";
    }
}