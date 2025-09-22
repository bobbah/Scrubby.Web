using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models;

namespace Scrubby.Web.Controllers;

public class IconController : Controller
{
    [HttpGet("icon/search/{searchTerm?}")]
    public IActionResult SearchIcon(string searchTerm, [FromQuery] string codebase) => View("SearchResult",
        new IconSearchResultModel { SearchQuery = searchTerm, Codebase = codebase ?? "tgstation" });

    [HttpPost("iconsearch")]
    [ValidateAntiForgeryToken]
    public IActionResult SearchIconPost([FromForm] string query) =>
        RedirectToAction("SearchIcon", new { searchTerm = query });

    [HttpGet("/dmi/{*path}")]
    public IActionResult GetSpriteSheet(string path, [FromQuery] string codebase) => View("SpriteSheet",
        new DMIViewModel { Path = path, Codebase = codebase ?? "tgstation" });
}