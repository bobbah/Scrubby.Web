using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.ViewComponents;

public class AnnouncementViewComponent : ViewComponent
{
    private readonly IAnnouncementService _announcements;

    public AnnouncementViewComponent(IAnnouncementService announce)
    {
        _announcements = announce;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<ScrubbyAnnouncement> announcements;
        try
        {
            announcements = await _announcements.GetAnnouncements();
        }
        catch
        {
            // TODO: log failed to get announcements
            // This can occur when the DB is unavailable
            announcements = new List<ScrubbyAnnouncement>();
        }
        
        return View(announcements);
    }
}