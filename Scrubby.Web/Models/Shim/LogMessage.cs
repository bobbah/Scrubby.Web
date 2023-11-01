using System;

namespace Scrubby.Web.Models.Shim;

public class LogMessage
{
    public DateTime? Timestamp { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public SSVec Origin { get; set; }
    public string OriginName { get; set; }
    public int RelativeIndex { get; set; }

    public override string ToString()
    {
        if (Origin != null)
        {
            if (OriginName != null) return $"[{Timestamp}] {Message} ({OriginName} {Origin})";
            return $"[{Timestamp}] {Message} {Origin}";
        }

        return $"[{Timestamp}] {Message}";
    }
}