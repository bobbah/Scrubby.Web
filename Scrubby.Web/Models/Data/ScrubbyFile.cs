using System;

namespace Scrubby.Web.Models.Data;

public class ScrubbyFile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Round { get; set; }
    public int Size { get; set; }
    public DateTimeOffset Uploaded { get; set; }
    public bool Processed { get; set; }
    public bool Failed { get; set; }
    public bool Decomposed { get; set; }
    public Guid StockpileId { get; set; }
}