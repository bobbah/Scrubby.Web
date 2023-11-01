namespace Scrubby.Web.Models.PostRequests;

public class ReceiptRetrievalModel
{
    public string CKey { get; set; }
    public int Limit { get; set; }
    public int StartingRound { get; set; }
}