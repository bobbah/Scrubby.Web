using System;

namespace Scrubby.Web.Models.Data;

public class ScrubbyAnnouncement
{
    public int Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string Message { get; set; }
    public bool Active { get; set; }
}