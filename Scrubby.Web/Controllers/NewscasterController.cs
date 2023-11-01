using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class NewscasterController : Controller
{
    private readonly INewscasterService _newscaster;

    public NewscasterController(INewscasterService newscaster)
    {
        _newscaster = newscaster;
    }

    [HttpGet("round/{roundID}/newscaster")]
    public async Task<IActionResult> GetRound(int roundID)
    {
        var model = await _newscaster.GetRound(roundID);
        return model == null ? NotFound() : View(model);
    }
}