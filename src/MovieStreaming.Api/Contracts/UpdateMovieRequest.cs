using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Api.Contracts;

public sealed record UpdateMovieRequest(
    int DurationMinutes,
    List<Genre> Genres,
    string Language,
    decimal Rating,
    int Seasons,
    decimal RentalCost);
