using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubby.Web.Models.Shim;

public class Runtime
{
    public int Round { get; set; }
    public DateTime Timestamp { get; set; }
    public string Exception { get; set; }
    public ProcCall ExceptionAt { get; set; }
    public List<ProcTrace> Stacktrace { get; set; }
}

public class ProcCall
{
    public string Proc { get; set; }
    public string ProcPath { get; set; }
    public string SourceFile { get; set; }
    public int Line { get; set; }
    public ByondAtom Source { get; set; }
    public ByondAtom SourceLocation { get; set; }
    public ByondAtom User { get; set; }
    public ByondAtom UserLocation { get; set; }
}

public class ProcTrace
{
    public ByondAtom Source { get; set; }
    public string ProcName { get; set; }
    public List<string> Args { get; set; }

    public override string ToString()
    {
        var toReturn = Source != null ? $"{Source} : " : "";
        if (ProcName != null) toReturn += $"{ProcName}({string.Join(", ", Args.Select(x => x ?? "NULL"))})";
        return toReturn;
    }
}

public class ByondAtom
{
    public string Name { get; set; }
    public string TypePath { get; set; }
    public SSVec Location { get; set; }

    public override string ToString()
    {
        var toReturn = Name ?? "";
        if (TypePath != null) toReturn += $" ({TypePath})";
        if (Location != null) toReturn += $" {Location}";
        return toReturn.Trim();
    }
}

public class RawRuntimeData
{
    public List<string> Lines { get; set; }
    public List<string> Callstack { get; set; }
    public Dictionary<string, string> KVP { get; set; }
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return $"[{Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Len: {Lines.Count}";
    }
}