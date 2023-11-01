using System;
using NodaTime;

namespace Scrubby.Web.Stockpile;

public record ObjectMetadata
{
    public Guid Id { get; init; }
    public long Size { get; init; }
    public string? Name { get; init; }
    public Instant Created { get; init; }
}

