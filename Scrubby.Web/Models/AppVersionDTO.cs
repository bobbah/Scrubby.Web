using Scrubby.Web.Version;

namespace Scrubby.Web.Models;

public record AppVersionDTO(string Version, string Commit, string CopyrightNotice)
{
    internal AppVersionDTO(AssemblyInformation assemblyInfo) : this(assemblyInfo.Version, assemblyInfo.Commit,
        assemblyInfo.CopyrightNotice)
    {
    }
}