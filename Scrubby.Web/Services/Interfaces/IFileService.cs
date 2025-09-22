using System.Threading.Tasks;
using Scrubby.Web.Models.Data;

namespace Scrubby.Web.Services.Interfaces;

public interface IFileService
{
    public Task<ScrubbyFile> GetFile(int id);
}