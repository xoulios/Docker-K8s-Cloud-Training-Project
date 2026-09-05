using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Api.Contracts;

public sealed record CreateMovieRequest(
    string Code,
    string Title,
    string Director,
    int Year,
    int DurationMinutes,
    List<Genre> Genres,
    string Language,
    decimal Rating,
    int Seasons,
    decimal RentalCost);
