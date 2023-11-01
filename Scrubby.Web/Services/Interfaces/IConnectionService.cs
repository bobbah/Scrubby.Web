using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Services.Interfaces;

public interface IConnectionService
{
    Task<List<ServerRoundStatistic>> GetConnectionStatsForCKey(CKey ckey, DateTime startDate);
    Task<List<ServerConnection>> GetConnectionsForRound(int round, IEnumerable<string> ckeys = null);
}