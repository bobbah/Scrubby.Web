using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Scrubby.Web.Models.Data;
using Scrubby.Web.Models.PostRequests;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Services.Interfaces;

public interface IFileService
{
    public Task<IEnumerable<LogMessage>> GetMessages(FileMessagePostModel model);
    public Task<ScrubbyFile> GetFile(int id);
}