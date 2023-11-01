using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

[Authorize(Roles = "Developer")]
public class AnnouncementController : Controller
{
    private readonly IAnnouncementService _announcements;

    public AnnouncementController(IAnnouncementService announcements)
    {
        _announcements = announcements;
    }

    [HttpGet("admin/announcements")]
    public IActionResult ManageAnnouncements()
    {
        return View("../Admin/ManageAnnouncements");
    }

    [HttpGet("api/announcement")]
    public async Task<IActionResult> GetAnnouncements()
    {
        return Ok(await _announcements.GetAnnouncements(false));
    }

    [HttpPost("api/announcement")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] ScrubbyAnnouncement announcement)
    {
        var result = await _announcements.CreateAnnouncement(announcement);
        return result != null ? Ok(result) : BadRequest("Could not create announcement as submitted");
    }

    [HttpPut("api/announcement")]
    public async Task<IActionResult> UpdateAnnouncement([FromBody] ScrubbyAnnouncement announcement)
    {
        var result = await _announcements.UpdateAnnouncement(announcement);
        return result != null ? Ok(result) : BadRequest("Could not update announcement as submitted");
    }

    [HttpDelete("api/announcement")]
    public async Task<IActionResult> DeleteAnnouncement([FromBody] ScrubbyAnnouncement announcement)
    {
        return await _announcements.DeleteAnnouncement(announcement) ? Ok() : NotFound();
    }
}