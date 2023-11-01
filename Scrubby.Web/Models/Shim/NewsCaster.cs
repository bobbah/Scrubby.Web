using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Scrubby.Web.Models.Shim;

public class NewsCasterChannel
{
    [JsonPropertyName("channel name")] public string Name { get; set; }
    public string Author { get; set; }
    public bool Censored { get; set; }
    [JsonPropertyName("author censored")] public bool AuthorCensored { get; set; }
    public List<NewsCasterMessage> Messages { get; set; }
    public int Round { get; set; }
}

public class NewsCasterMessage
{
    public string Author { get; set; }
    [JsonPropertyName("time stamp")] public StationTime TimeStamp { get; set; }
    public bool Censored { get; set; }
    [JsonPropertyName("author censored")] public bool AuthorCensored { get; set; }
    [JsonPropertyName("photo file")] public string PhotoFile { get; set; }
    [JsonPropertyName("photo caption")] public string PhotoCaption { get; set; }
    public string Body { get; set; }
    public List<NewsCasterComment> Comments { get; set; }
}

public class NewsCasterComment
{
    public string Author { get; set; }
    [JsonPropertyName("time stamp")] public StationTime TimeStamp { get; set; }
    public string Body { get; set; }
}

public class NewsCasterWanted
{
    public string Author { get; set; }
    public string Criminal { get; set; }
    public string Description { get; set; }
    [JsonPropertyName("photo file")] public string PhotoFile { get; set; }
    public int Round { get; set; }
}