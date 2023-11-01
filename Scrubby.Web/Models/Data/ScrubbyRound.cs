using System;
using System.Collections.Generic;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models.Data;

public class ScrubbyRound
{
    public int Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string Server { get; set; }
    public string ServerInternal { get; set; }
    public string Map { get; set; }
    public int MapLoadingSeconds { get; set; }
    public float GameStartDuration { get; set; }
    public DateTimeOffset? GameStartTime { get; set; }

    public string BaseURL { get; set; }
    public RoundBuildInfo VersionInfo { get; set; }
    public List<ScrubbyFile> Files { get; set; }
    public List<Round.RoundPlayer> Players { get; set; }
    public List<ProcessStatus> Status { get; set; }
}