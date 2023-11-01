using System;

namespace Scrubby.Web.Models.Api;

public class FailedRequest
{
    public string Message { get; set; }
    public string Detail { get; set; }
    public DateTime Timestamp { get; set; }
}