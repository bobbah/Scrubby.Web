using System;

namespace Scrubby.Web.Stockpile;

public record ArchiveObjectMetadata : ObjectMetadata
{
    public Guid ParentArchive { get; init; }
}