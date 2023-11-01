using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _users;

    public UserController(IUserService users)
    {
        _users = users;
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        return View("User");
    }
}