using System.Collections.Generic;

namespace Scrubby.Web.Models.Shim;

public class RoundBuildInfo
{
    public string Master { get; set; }
    public List<TestMerge> TestMerges { get; set; }
    public string Head { get; set; }
}

public class TestMerge
{
    public int PR { get; set; }
    public string Commit { get; set; }
}