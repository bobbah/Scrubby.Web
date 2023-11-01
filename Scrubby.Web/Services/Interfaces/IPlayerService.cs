using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Scrubby.Web.Models;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Services.Interfaces;

public interface IPlayerService
{
    Task<List<PlayerNameStatistic>> SearchForCKey(Regex regex);
    Task<List<PlayerNameStatistic>> SearchForICName(Regex regex);
    Task<List<RoundReceipt>> GetRoundReceiptsForPlayer(CKey ckey, int startingRound, int limit);

    Task<List<RoundReceipt>> GetRoundReceiptsForPlayer(CKey ckey, DateTime? startDate = null,
        DateTime? endDate = null);
}