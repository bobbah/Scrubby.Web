using System.Collections.Generic;

namespace Scrubby.Web.Models;

public class NameSearchResultModel
{
    public List<PlayerNameStatistic> Statistics { get; set; }
    public string SearchTerm { get; set; }
}