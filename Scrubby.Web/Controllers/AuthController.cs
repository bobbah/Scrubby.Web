using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Scrubby.Web.Controllers;

public class AuthController : Controller
{
    [HttpGet("/login")]
    public async Task<IActionResult> Login()
    {
        if (User.Identity is not { IsAuthenticated: true })
            await HttpContext.ChallengeAsync();
        return RedirectToAction("Me", "User");
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}