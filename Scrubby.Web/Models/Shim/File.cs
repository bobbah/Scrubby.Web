using System;

namespace Scrubby.Web.Models.Shim;

public class File
{
    public string FileName { get; set; }
    public bool Parsed { get; set; }
    public bool Failed { get; set; }
    public DateTime? UploadDate { get; set; }
    public long Length { get; set; } = -1;
}