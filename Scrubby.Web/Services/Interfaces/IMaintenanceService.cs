using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubby.Web.Services.Interfaces;

public interface IMaintenanceService
{
    Task ReparseRoundsAsync(IEnumerable<int> rounds, bool redownloadFiles);
    Task ReparseRoundsWithFileAsync(string fileName);
    Task<IEnumerable<Guid>> GetObjectsForArchiveDiscovery(int round);
}