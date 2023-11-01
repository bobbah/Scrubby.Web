using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _users;

    public AuthController(IUserService users)
    {
        _users = users;
    }


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