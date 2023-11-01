using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class SearchController : Controller
{
    private readonly IPlayerService _players;

    public SearchController(IPlayerService players)
    {
        _players = players;
    }

    [HttpGet("search/player")]
    public IActionResult PlayerIndex()
    {
        return View("PlayerIndex");
    }

    [HttpPost("api/search/player")]
    public async Task<IActionResult> ExecutePlayerSearch([FromBody] PlayerSearchPostModel model)
    {
        if (model == null || model.SearchType == PlayerSearchType.Unknown || model.Regex == null)
            return StatusCode(400, "Bad request");
        if (string.IsNullOrWhiteSpace(model.Regex.ToString()))
            return StatusCode(400, "Regex pattern cannot be simply empty or whitespace.");
        if (model.Regex.ToString().Length > 50)
            return StatusCode(400, "Regex pattern too long, restrict to between 3-50 characters.");
        if (model.Regex.ToString().Length < 3)
            return StatusCode(400, "Regex pattern too short, restrict to between 3-50 characters.");

        if (model.SearchType == PlayerSearchType.ICName)
            return Ok(await _players.SearchForICName(model.Regex));
        return Ok(await _players.SearchForCKey(model.Regex));
    }
}