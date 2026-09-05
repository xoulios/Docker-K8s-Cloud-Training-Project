using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Domain.Queries;

public sealed record MovieSearchQuery(
    string? Code = null,
    string? Title = null,
    string? Director = null,
    int? Year = null,
    int? DurationMinutes = null,
    Genre? Genre = null,
    string? Language = null,
    decimal? Rating = null,
    int? Seasons = null,
    decimal? RentalCost = null);
