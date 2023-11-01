namespace Scrubby.Web.Models;

public record ErrorModel
{
    public int StatusCode { get; init; }
    public string TraceIdentifier { get; init; }
}