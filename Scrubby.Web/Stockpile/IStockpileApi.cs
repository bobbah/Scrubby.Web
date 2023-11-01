using System;
using System.IO;
using System.Threading.Tasks;
using RestEase;

namespace Scrubby.Web.Stockpile;

public interface IStockpileApi
{
    [Header("XApiKey")]
    string ApiKey { get; set; }
    
    [Get("object/{id}")]
    Task<Stream> GetObject([Path] Guid id);
    
    [Get("archive/{id}/delete")]
    Task DeleteArchive([Path] Guid id);
    
    [Get("object/{id}/metadata")]
    Task<ArchiveObjectMetadata> GetObjectMetadata([Path] Guid id);
}