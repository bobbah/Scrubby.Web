using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Models.Shim;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class CKeyController(
    IPlayerService players,
    IConnectionService connService,
    ICKeyService ckeyService)
    : Controller
{
    [HttpGet("ckey/{ckey}")]
    public async Task<IActionResult> FetchCKey(string ckey)
    {
        var toGive = new CKeyModel
        {
            Key = new CKey(ckey)
        };

        var dataFetch = new Task[]
        {
            Task.Run(async () => toGive.Names = await ckeyService.GetNamesForCKeyAsync(toGive.Key)),
            Task.Run(async () => toGive.Playtime = await ckeyService.GetServerCountForCKeyAsync(toGive.Key)),
            Task.Run(async () => toGive.ByondKey = await ckeyService.GetByondKeyAsync(toGive.Key))
        };
        await Task.WhenAll(dataFetch);

        if (toGive.Playtime.Count == 0)
            return NotFound();

        return View("View", toGive);
    }
    
    [HttpGet("receipts/{ckey}")]
    public async Task<IActionResult> GetReceiptsForCKey(string ckey)
    {
        var toSearch = new CKey(ckey);
        var result = await players.GetRoundReceiptsForPlayer(toSearch);
        return Ok(result);
    }

    [HttpPost("ckey/{ckey}/connections")]
    public async Task<IActionResult> GetConnectionData(string ckey, int length = 180)
    {
        if (string.IsNullOrEmpty(ckey)) return NotFound("Failed to find ckey, or invalid ckey");

        var toSearch = new CKey(ckey);
        var result =
            await connService.GetConnectionStatsForCKey(toSearch, DateTime.UtcNow.AddDays(-1 * Math.Abs(length)));
        return Ok(result);
    }

    [HttpPost("ckey/{ckey}/receipts")]
    public async Task<IActionResult> GetReceipts([FromBody] ReceiptRetrievalModel model)
    {
        if (string.IsNullOrEmpty(model.CKey)) return NotFound("Failed to find ckey, or invalid ckey");

        var toSearch = new CKey(model.CKey);
        var result = await players.GetRoundReceiptsForPlayer(toSearch, model.StartingRound, model.Limit);
        return Ok(result);
    }
}