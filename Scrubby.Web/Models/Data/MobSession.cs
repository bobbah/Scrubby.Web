using System;

namespace Scrubby.Web.Models.Data;

public class MobSession
{
    public int Id { get; set; }
    public int Round { get; set; }
    public string CKey { get; set; }
    public long StartEvidence { get; set; }
    public long EndEvidence { get; set; }
    public string MobType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}