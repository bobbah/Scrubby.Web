using System;
using System.Collections.Generic;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models.Data;

public class ImprovedRuntime
{
    private string _procPath;
    public DateTime Timestamp { get; set; }
    public string Exception { get; set; }
    public string Proc { get; private set; }

    public string ProcPath
    {
        get => _procPath;
        set
        {
            _procPath = value;
            Proc = _procPath.Split('/')[^1];
        }
    }

    public string SourceFile { get; set; }
    public int Line { get; set; }
    public ImprovedAtomReference Source { get; set; }
    public ImprovedAtomReference SourceLocation { get; set; }
    public ImprovedAtomReference User { get; set; }
    public ImprovedAtomReference UserLocation { get; set; }
    public List<ImprovedStacktrace> Stacktrace { get; set; }

    public static ImprovedRuntime FromRuntime(Runtime source)
    {
        var toReturn = new ImprovedRuntime
        {
            Timestamp = source.Timestamp,
            Exception = source.Exception,
            ProcPath = source.ExceptionAt.ProcPath,
            SourceFile = source.ExceptionAt.SourceFile,
            Line = source.ExceptionAt.Line,
            Source = ImprovedAtomReference.FromByondAtom(source.ExceptionAt.Source),
            SourceLocation = ImprovedAtomReference.FromByondAtom(source.ExceptionAt.SourceLocation),
            User = ImprovedAtomReference.FromByondAtom(source.ExceptionAt.User),
            UserLocation = ImprovedAtomReference.FromByondAtom(source.ExceptionAt.UserLocation),
            Stacktrace = new List<ImprovedStacktrace>()
        };

        // Add all stacktrace lines
        for (var i = 0; i < source.Stacktrace.Count; i++)
            toReturn.Stacktrace.Add(new ImprovedStacktrace
            {
                Index = i,
                Line = source.Stacktrace[i].ToString()
            });

        return toReturn;
    }
}