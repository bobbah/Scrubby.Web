using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.Api;
using Scrubby.Web.Models.CommonRounds;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

[Authorize(Roles = "Developer,In-Game Admin,BetaTester")]
public class InvestigateController(
    IPlayerService playerService,
    IRoundService roundService,
    IConnectionService connectionService)
    : Controller
{
    [HttpGet("investigate")]
    public IActionResult Index() => View();

    [HttpGet("investigate/connections")]
    public IActionResult InvestigateConnections() => View();

    [HttpGet("investigate/suicides")]
    public IActionResult InvestigateSuicides() => View();

    [HttpPost("api/roundsforckeys")]
    public async Task<IActionResult> GetRoundsForCKeys([FromBody] RoundsForCKeyAggregationModel model)
    {
        if (model.CKeys == null || model.CKeys.Count == 0) return new StatusCodeResult(400);

        return Json(await roundService.GetCommonRounds(model.CKeys, new CommonRoundsOptions
        {
            StartingRound = model.StartingRound,
            GTERound = model.GTERound,
            Limit = model.Limit
        }));
    }

    [HttpPost("api/connections/round")]
    public async Task<IActionResult> GetConnectionsForRound([FromBody] ConnectionsForRoundAggregationModel model) => Json(await connectionService.GetConnectionsForRound(model.Round, model.CKeyFilter));

    [HttpPost("api/receipts")]
    public async Task<IActionResult> GetReceiptsForPlayer([FromBody] ReceiptsForPlayerPostModel model) =>
        Ok(await playerService.GetRoundReceiptsForPlayer(new CKey(model.CKey), model.StartDate,
            model.EndDate));
}