using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubby.Web.Models.Data;

namespace Scrubby.Web.Services.Interfaces;

public interface IAnnouncementService
{
    Task<List<ScrubbyAnnouncement>> GetAnnouncements(bool onlyActive = true);
    Task<ScrubbyAnnouncement> CreateAnnouncement(ScrubbyAnnouncement announcement);
    Task<ScrubbyAnnouncement> UpdateAnnouncement(ScrubbyAnnouncement announcement);
    Task<bool> DeleteAnnouncement(ScrubbyAnnouncement announcement);
}