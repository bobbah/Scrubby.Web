using System;
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
public class InvestigateController : Controller
{
    private readonly IConnectionService _connectionService;
    private readonly IPlayerService _playerService;
    private readonly IRoundService _roundService;

    public InvestigateController(IPlayerService playerService, IRoundService roundService,
        IConnectionService connectionService)
    {
        _playerService = playerService;
        _roundService = roundService;
        _connectionService = connectionService;
    }

    [HttpGet("investigate")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("investigate/connections")]
    public IActionResult InvestigateConnections()
    {
        return View();
    }

    [HttpGet("investigate/suicides")]
    public IActionResult InvestigateSuicides()
    {
        return View();
    }

    [HttpPost("api/roundsforckeys")]
    public async Task<IActionResult> GetRoundsForCKeys([FromBody] RoundsForCKeyAggregationModel model)
    {
        if (model.CKeys == null || model.CKeys.Count == 0) return new StatusCodeResult(400);

        return Json(await _roundService.GetCommonRounds(model.CKeys, new CommonRoundsOptions
        {
            StartingRound = model.StartingRound,
            GTERound = model.GTERound,
            Limit = model.Limit
        }));
    }

    [HttpPost("api/connections/round")]
    public async Task<IActionResult> GetConnectionsForRound([FromBody] ConnectionsForRoundAggregationModel model)
    {
        return Json(await _connectionService.GetConnectionsForRound(model.Round, model.CKeyFilter));
    }

    [HttpPost("api/receipts")]
    public async Task<IActionResult> GetReceiptsForPlayer([FromBody] ReceiptsForPlayerPostModel model)
    {
        return Ok(await _playerService.GetRoundReceiptsForPlayer(new CKey(model.CKey), model.StartDate,
            model.EndDate));
    }
}