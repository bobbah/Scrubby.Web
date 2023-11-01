using System;
using System.Linq;
using System.Reflection;

namespace Scrubby.Web.Version;

public record AssemblyInformation(string Version, string Commit, string CopyrightNotice)
{
    public static readonly AssemblyInformation Current = new(typeof(AssemblyInformation).Assembly);

    private AssemblyInformation(Assembly assembly) : this(
        assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "MinVerVersion")
            ?.Value,
        assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "SourceRevisionId")
            ?.Value,
        assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright)
    {
    }

    public ReadOnlySpan<char> GetBuildCommit(int maxHashLength = 7)
    {
        if (maxHashLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maxHashLength), "Hash length cannot be less than zero!");
        if (string.IsNullOrEmpty(Commit))
            return null;

        var commitSpan = Commit.AsSpan();
        return commitSpan[..(Math.Clamp(maxHashLength, 1, commitSpan.Length) - 1)];
    }
}