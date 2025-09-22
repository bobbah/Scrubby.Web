using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class HomeController(IScrubbyService scrubby) : Controller
{
    public async Task<IActionResult> Index() => View(await scrubby.GetBasicStats());
}