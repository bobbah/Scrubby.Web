using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class FileController : Controller
{
    private readonly IFileService _files;
    private readonly IRoundService _rounds;

    public FileController(IRoundService rounds, IFileService files)
    {
        _rounds = rounds;
        _files = files;
    }

    [HttpGet("round/{roundid:int}/files")]
    public async Task<IActionResult> FetchFile(int roundid, [FromQuery(Name = "file")] string[] files,
        [FromQuery(Name = "ckey")] string[] ckeys, [FromQuery(Name = "range")] string[] ranges)
    {
        var round = await _rounds.GetRound(roundid);
        if (round == null || files == null || files.Length == 0) return NotFound();
        files = files.Select(x => x.ToLower()).Where(x => round.Files.Any(y => y.Name == x)).Distinct()
            .ToArray();

        var link = $"https://scrubby.melonmesa.com/round/{round.Id}/files?";
        foreach (var file in files) link = $"{link}file={file}&";
        if (ckeys != null && ckeys.Length > 0)
            foreach (var ckey in ckeys)
                link = $"{link}ckey={ckey}&";
        if (ranges != null && ranges.Length > 0)
            foreach (var range in ranges)
                link = $"{link}range={range}&";

        var model = new LogModel
        {
            Parent = round,
            Data = new FileMessagePostModel
            {
                RoundID = roundid,
                CKeys = ckeys,
                Files = files,
                Ranges = ranges
            }
        };

        return View("View", model);
    }

    [HttpPost("api/file/messages")]
    public async Task<IActionResult> FetchMessages([FromBody] FileMessagePostModel model)
    {
        var result = await _files.GetMessages(model);
        return Ok(result);
    }
}