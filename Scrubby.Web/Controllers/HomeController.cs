using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class HomeController : Controller
{
    private readonly IScrubbyService _scrubby;

    public HomeController(IScrubbyService scrubby)
    {
        _scrubby = scrubby;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _scrubby.GetBasicStats());
    }
}