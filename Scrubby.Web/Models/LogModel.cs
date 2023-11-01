using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.PostRequests;

namespace Scrubby.Web.Models;

public class LogModel
{
    public ScrubbyRound Parent { get; set; }
    public FileMessagePostModel Data { get; set; }
}