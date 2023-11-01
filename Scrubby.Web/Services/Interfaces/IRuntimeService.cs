using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models.Data;

namespace Scrubby.Web.Services.Interfaces;

public interface IRuntimeService
{
    Task<IEnumerable<ImprovedRuntime>> GetRuntimesForRound(int roundID);
}