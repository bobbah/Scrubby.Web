using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class CKeyController : Controller
{
    private readonly ICKeyService _ckey;
    private readonly IConnectionService _connService;
    private readonly IPlayerService _players;
    private readonly ISuicideService _suicides;

    public CKeyController(IPlayerService players, ISuicideService suicides, IConnectionService connService,
        ICKeyService ckey)
    {
        _players = players;
        _suicides = suicides;
        _connService = connService;
        _ckey = ckey;
    }

    [HttpGet("ckey/{ckey}")]
    public async Task<IActionResult> FetchCKey(string ckey)
    {
        var toGive = new CKeyModel
        {
            Key = new CKey(ckey)
        };

        var dataFetch = new Task[]
        {
            Task.Run(async () => toGive.Names = await _ckey.GetNamesForCKeyAsync(toGive.Key)),
            Task.Run(async () => toGive.Playtime = await _ckey.GetServerCountForCKeyAsync(toGive.Key)),
            Task.Run(async () => toGive.ByondKey = await _ckey.GetByondKeyAsync(toGive.Key))
        };
        await Task.WhenAll(dataFetch);

        if (!toGive.Playtime.Any())
            return NotFound();

        return View("View", toGive);
    }

    [HttpGet("suicides/{ckey}")]
    public async Task<IActionResult> GetSuicidesForCKey(string ckey)
    {
        var toSearch = new CKey(ckey);
        var result = await _suicides.GetSuicidesForCKey(toSearch);
        return Ok(result);
    }

    [HttpGet("receipts/{ckey}")]
    public async Task<IActionResult> GetReceiptsForCKey(string ckey)
    {
        var toSearch = new CKey(ckey);
        var result = await _players.GetRoundReceiptsForPlayer(toSearch);
        return Ok(result);
    }

    [HttpPost("ckey/{ckey}/connections")]
    public async Task<IActionResult> GetConnectionData(string ckey, int length = 180)
    {
        if (string.IsNullOrEmpty(ckey)) return NotFound("Failed to find ckey, or invalid ckey");

        var toSearch = new CKey(ckey);
        var result =
            await _connService.GetConnectionStatsForCKey(toSearch, DateTime.UtcNow.AddDays(-1 * Math.Abs(length)));
        return Ok(result);
    }

    [HttpPost("ckey/{ckey}/receipts")]
    public async Task<IActionResult> GetReceipts([FromBody] ReceiptRetrievalModel model)
    {
        if (string.IsNullOrEmpty(model.CKey)) return NotFound("Failed to find ckey, or invalid ckey");

        var toSearch = new CKey(model.CKey);
        var result = await _players.GetRoundReceiptsForPlayer(toSearch, model.StartingRound, model.Limit);
        return Ok(result);
    }
}