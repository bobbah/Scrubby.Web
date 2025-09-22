using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Scrubby.Web.Controllers;

[Authorize]
public class UserController() : Controller
{
    [HttpGet("me")]
    public IActionResult Me() => View("User");
}