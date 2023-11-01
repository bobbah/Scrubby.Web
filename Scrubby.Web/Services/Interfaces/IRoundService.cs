using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models.CommonRounds;
using Scrubby.Web.Models.Data;

namespace Scrubby.Web.Services.Interfaces;

public interface IRoundService
{
    Task<ScrubbyRound> GetRound(int id);
    Task<int> GetNext(int id, bool forward = true, List<string> ckey = null);
    Task<List<CommonRoundModel>> GetCommonRounds(IEnumerable<string> ckeys, CommonRoundsOptions options = null);
}