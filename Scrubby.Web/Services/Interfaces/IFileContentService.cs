using System.IO;
using System.Threading.Tasks;

namespace Scrubby.Web.Services.Interfaces;

public interface IFileContentService
{
    public Task<Stream> GetFileContent(int id);
}