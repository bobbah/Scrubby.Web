using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

[Authorize(Roles = "Developer")]
public class AnnouncementController(IAnnouncementService announcements) : Controller
{
    [HttpGet("admin/announcements")]
    public IActionResult ManageAnnouncements() => View("../Admin/ManageAnnouncements");

    [HttpGet("api/announcement")]
    public async Task<IActionResult> GetAnnouncements() => Ok(await announcements.GetAnnouncements(false));

    [HttpPost("api/announcement")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] ScrubbyAnnouncement announcement)
    {
        var result = await announcements.CreateAnnouncement(announcement);
        return result != null ? Ok(result) : BadRequest("Could not create announcement as submitted");
    }

    [HttpPut("api/announcement")]
    public async Task<IActionResult> UpdateAnnouncement([FromBody] ScrubbyAnnouncement announcement)
    {
        var result = await announcements.UpdateAnnouncement(announcement);
        return result != null ? Ok(result) : BadRequest("Could not update announcement as submitted");
    }

    [HttpDelete("api/announcement")]
    public async Task<IActionResult> DeleteAnnouncement([FromBody] ScrubbyAnnouncement announcement) => await announcements.DeleteAnnouncement(announcement) ? Ok() : NotFound();
}