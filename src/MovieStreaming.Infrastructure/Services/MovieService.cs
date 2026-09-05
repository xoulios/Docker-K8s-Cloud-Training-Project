using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MovieStreaming.Domain.Common;
using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;
using MovieStreaming.Domain.Exceptions;
using MovieStreaming.Domain.Queries;
using MovieStreaming.Domain.Services;
using MovieStreaming.Infrastructure.Persistence;

namespace MovieStreaming.Infrastructure.Services;

public class MovieService(MovieDbContext dbContext) : IMovieService
{
    public async Task<Result<Movie>> CreateAsync(
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
        CancellationToken ct = default)
    {
        var createResult = Movie.Create(
            code, title, director, year, durationMinutes, genres, language, rating, seasons, rentalCost);

        if (createResult.IsFailure)
            return createResult;

        var movie = createResult.Value!;

        if (await dbContext.Movies.AnyAsync(m => m.Code == movie.Code, ct))
            throw new DuplicateMovieException($"A movie with code '{movie.Code}' already exists.");

        if (await dbContext.Movies.AnyAsync(
                m => m.Title == movie.Title && m.Year == movie.Year && m.Director == movie.Director, ct))
            throw new DuplicateMovieException(
                "A movie with the same title, year, and director already exists.");

        dbContext.Movies.Add(movie);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new DuplicateMovieException(
                "A movie with the same code or the same title/year/director combination already exists.");
        }

        return createResult;
    }

    public async Task<Movie> GetByCodeAsync(string code, CancellationToken ct = default)
        => await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Code == code, ct)
           ?? throw new MovieNotFoundException(code);

    public async Task<IReadOnlyList<Movie>> SearchAsync(MovieSearchQuery query, CancellationToken ct = default)
    {
        var moviesQuery = dbContext.Movies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
            moviesQuery = moviesQuery.Where(m => m.Code == query.Code);
        if (!string.IsNullOrWhiteSpace(query.Title))
            moviesQuery = moviesQuery.Where(m => EF.Functions.Like(m.Title, $"%{query.Title}%"));
        if (!string.IsNullOrWhiteSpace(query.Director))
            moviesQuery = moviesQuery.Where(m => EF.Functions.Like(m.Director, $"%{query.Director}%"));
        if (query.Year.HasValue)
            moviesQuery = moviesQuery.Where(m => m.Year == query.Year);
        if (query.DurationMinutes.HasValue)
            moviesQuery = moviesQuery.Where(m => m.DurationMinutes == query.DurationMinutes);
        if (!string.IsNullOrWhiteSpace(query.Language))
            moviesQuery = moviesQuery.Where(m => EF.Functions.Like(m.Language, $"%{query.Language}%"));
        if (query.Rating.HasValue)
            moviesQuery = moviesQuery.Where(m => m.Rating == query.Rating);
        if (query.Seasons.HasValue)
            moviesQuery = moviesQuery.Where(m => m.Seasons == query.Seasons);
        if (query.RentalCost.HasValue)
            moviesQuery = moviesQuery.Where(m => m.RentalCost == query.RentalCost);

        var movies = await moviesQuery.ToListAsync(ct);

        if (query.Genre.HasValue)
            movies = movies.Where(m => m.Genres.Contains(query.Genre.Value)).ToList();

        return movies
            .OrderBy(m => m.Genres.Select(g => g.ToString()).OrderBy(g => g, StringComparer.Ordinal).FirstOrDefault())
            .ThenBy(m => m.Director, StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.Year)
            .ToList();
    }

    public async Task DeleteAsync(string code, CancellationToken ct = default)
    {
        var movie = await dbContext.Movies.FirstOrDefaultAsync(m => m.Code == code, ct)
            ?? throw new MovieNotFoundException(code);

        dbContext.Movies.Remove(movie);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Result> UpdateAsync(
        string code,
        int durationMinutes,
        IEnumerable<Genre> genres,
        string language,
        decimal rating,
        int seasons,
        decimal rentalCost,
        CancellationToken ct = default)
    {
        var movie = await dbContext.Movies.FirstOrDefaultAsync(m => m.Code == code, ct)
            ?? throw new MovieNotFoundException(code);

        var updateResult = movie.Update(durationMinutes, genres, language, rating, seasons, rentalCost);
        if (updateResult.IsFailure)
            return updateResult;

        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}
