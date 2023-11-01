using System;

namespace Scrubby.Web.Models.Shim;

public class Suicide
{
    public int RoundID { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan RelativeTime { get; set; }

    public double RelativeMinutes => RelativeTime.TotalMinutes;
    public CKey CKey { get; set; }
    public string OriginName { get; set; }
    public SSVec Origin { get; set; }
    public bool RoundStart { get; set; }
    public bool Antagonist { get; set; }

    public override string ToString()
    {
        return
            $"[{RoundID}] {CKey.Raw} [{CKey.Cleaned}] killed themselves at {Timestamp} [R: {RelativeTime}] ({OriginName} {Origin})";
    }
}