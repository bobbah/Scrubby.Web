using System;
using Scrubby.Web.Models;
using Scrubby.Web.Services.Interfaces;
using Scrubby.Web.Version;

namespace Scrubby.Web.Services;

public class AppStatusService : IAppStatusService
{
    private static readonly AppVersionDTO VersionDTO = new(AssemblyInformation.Current);

    public AppStatusService()
    {
    }

    /// <inheritdoc />
    public ReadOnlySpan<char> GetVersion() => AssemblyInformation.Current.Version;

    /// <inheritdoc />
    public ReadOnlySpan<char> GetBuildCommit(int maxHashLength = 7) =>
        AssemblyInformation.Current.GetBuildCommit(maxHashLength);

    /// <inheritdoc />
    public ReadOnlySpan<char> GetCopyrightNotice() => AssemblyInformation.Current.CopyrightNotice;

    /// <inheritdoc />
    public AppVersionDTO GetAppVersionDTO() => VersionDTO;
}