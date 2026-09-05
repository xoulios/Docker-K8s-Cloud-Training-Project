using Microsoft.AspNetCore.Mvc;
using MovieStreaming.Api.Contracts;
using MovieStreaming.Domain.Queries;
using MovieStreaming.Domain.Services;

namespace MovieStreaming.Api.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateMovieRequest request, CancellationToken ct)
    {
        var result = await movieService.CreateAsync(
            request.Code, request.Title, request.Director, request.Year, request.DurationMinutes,
            request.Genres, request.Language, request.Rating, request.Seasons, request.RentalCost, ct);

        if (result.IsFailure)
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest, title: "Invalid movie data");

        var movie = result.Value!;
        return CreatedAtAction(nameof(GetByCode), new { code = movie.Code }, MovieResponse.FromDomain(movie));
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] MovieSearchQuery query, CancellationToken ct)
    {
        var movies = await movieService.SearchAsync(query, ct);
        return Ok(movies.Select(MovieResponse.FromDomain));
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var movie = await movieService.GetByCodeAsync(code, ct);
        return Ok(MovieResponse.FromDomain(movie));
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, UpdateMovieRequest request, CancellationToken ct)
    {
        var result = await movieService.UpdateAsync(
            code, request.DurationMinutes, request.Genres, request.Language,
            request.Rating, request.Seasons, request.RentalCost, ct);

        return result.IsSuccess
            ? NoContent()
            : Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest, title: "Invalid movie data");
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code, CancellationToken ct)
    {
        await movieService.DeleteAsync(code, ct);
        return NoContent();
    }
}
