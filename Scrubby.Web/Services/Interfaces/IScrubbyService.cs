using System.Threading.Tasks;
using Scrubby.Web.Models;

namespace Scrubby.Web.Services.Interfaces;

public interface IScrubbyService
{
    public Task<BasicStatsModel> GetBasicStats();
}