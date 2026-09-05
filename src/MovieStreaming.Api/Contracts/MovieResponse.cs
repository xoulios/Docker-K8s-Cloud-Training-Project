using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Api.Contracts;

public sealed record MovieResponse(
    string Code,
    string Title,
    string Director,
    int Year,
    int DurationMinutes,
    IReadOnlyCollection<Genre> Genres,
    string Language,
    decimal Rating,
    int Seasons,
    decimal RentalCost)
{
    public static MovieResponse FromDomain(Movie movie) => new(
        movie.Code,
        movie.Title,
        movie.Director,
        movie.Year,
        movie.DurationMinutes,
        movie.Genres,
        movie.Language,
        movie.Rating,
        movie.Seasons,
        movie.RentalCost);
}
