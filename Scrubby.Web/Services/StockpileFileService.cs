using System;
using System.IO;
using System.Threading.Tasks;
using Scrubby.Web.Services.Interfaces;
using Scrubby.Web.Stockpile;

namespace Scrubby.Web.Services;

public class StockpileFileService : IFileContentService
{
    private readonly IStockpileApi _stockpile;
    private readonly IFileService _file;

    public StockpileFileService(IStockpileApi stockpile, IFileService file)
    {
        _stockpile = stockpile;
        _file = file;
    }
    
    public async Task<Stream> GetFileContent(int id)
    {
        var fileMetadata = await _file.GetFile(id);
        if (fileMetadata == null)
            throw new Exception($"Failed to get file metadata when retrieving file content: {id}");
        
        var ms = new MemoryStream();
        await (await _stockpile.GetObject(fileMetadata.StockpileId)).CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}