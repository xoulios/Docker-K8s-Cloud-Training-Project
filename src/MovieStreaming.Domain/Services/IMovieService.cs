using MovieStreaming.Domain.Common;
using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;
using MovieStreaming.Domain.Queries;

namespace MovieStreaming.Domain.Services;

public interface IMovieService
{
    Task<Result<Movie>> CreateAsync(
        string code,
        string title,
        string director,
        int year,
        int durationMinutes,
        IEnumerable<Genre> genres,
        string language,
        decimal rating,
        int seasons,
        decimal rentalCost,
        CancellationToken ct = default);

    Task<Movie> GetByCodeAsync(string code, CancellationToken ct = default);

    Task<IReadOnlyList<Movie>> SearchAsync(MovieSearchQuery query, CancellationToken ct = default);

    Task DeleteAsync(string code, CancellationToken ct = default);

    Task<Result> UpdateAsync(
        string code,
        int durationMinutes,
        IEnumerable<Genre> genres,
        string language,
        decimal rating,
        int seasons,
        decimal rentalCost,
        CancellationToken ct = default);
}
