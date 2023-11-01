using System;

namespace Scrubby.Web.Models.Shim;

public enum Status
{
    Unknown,
    Waiting,
    Complete,
    Failed
}

public class ProcessStatus
{
    public Status Status { get; set; }
    public string Process { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}