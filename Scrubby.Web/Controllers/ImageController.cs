using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class ImageController : Controller
{
    private readonly IFileService _files;
    private readonly IFileContentService _fileContent;

    private readonly List<string> _validPictures = new()
    {
        "png",
        "jpeg",
        "jpg"
    };

    public ImageController(IFileService files, IFileContentService fileContent)
    {
        _files = files;
        _fileContent = fileContent;
    }

    [HttpGet("image/{id:int}")]
    public async Task<FileStreamResult> FetchImage(int id)
    {
        var f = await _files.GetFile(id);
        if (!_validPictures.Contains(f.Name.Split(".").Last()) || f.Size == 0) return null;
        var mime = MimeUtility.GetMimeMapping(f.Name);
        var stream = await _fileContent.GetFileContent(id);
        return new FileStreamResult(stream, mime);
    }
}