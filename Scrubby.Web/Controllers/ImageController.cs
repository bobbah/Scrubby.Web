using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Scrubby.Web.Services.Interfaces;

namespace Scrubby.Web.Controllers;

public class ImageController(IFileService files, IFileContentService fileContent) : Controller
{
    private readonly List<string> _validPictures =
    [
        "png",
        "jpeg",
        "jpg"
    ];

    [HttpGet("image/{id:int}")]
    public async Task<FileStreamResult> FetchImage(int id)
    {
        var f = await files.GetFile(id);
        if (!_validPictures.Contains(f.Name.Split(".").Last()) || f.Size == 0) return null;
        var mime = MimeUtility.GetMimeMapping(f.Name);
        var stream = await fileContent.GetFileContent(id);
        return new FileStreamResult(stream, mime);
    }
}