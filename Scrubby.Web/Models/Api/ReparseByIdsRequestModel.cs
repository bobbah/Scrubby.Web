namespace Scrubby.Web.Models.Api;

public class ReparseByIdsRequestModel
{
    public int[] Ids { get; set; }
    public bool DeleteFiles { get; set; }
}