namespace Scrubby.Web.Models.PostRequests;

public class FileMessagePostModel
{
    public int RoundID { get; set; }
    public string[] Files { get; set; }
    public string[] CKeys { get; set; }
    public string[] Ranges { get; set; }
}