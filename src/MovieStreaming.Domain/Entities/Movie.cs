using System.Text.RegularExpressions;
using MovieStreaming.Domain.Common;
using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Domain.Entities;

public sealed class Movie
{
    private static readonly Regex CodePattern = new("^[A-Za-z0-9]{10}$", RegexOptions.Compiled);
    private static readonly Regex TitlePattern = new(@"^[\p{L}\p{N}\s-]{2,}$", RegexOptions.Compiled);
    private static readonly Regex DirectorPattern = new(@"^[\p{L}\s]+$", RegexOptions.Compiled);
    private static readonly Regex LanguagePattern = new(@"^[\p{L}]{2,}$", RegexOptions.Compiled);

    private readonly List<Genre> _genres = [];

    private Movie() { }

    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Director { get; private set; } = null!;
    public int Year { get; private set; }
    public int DurationMinutes { get; private set; }
    public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();
    public string Language { get; private set; } = null!;
    public decimal Rating { get; private set; }
    public int Seasons { get; private set; }
    public decimal RentalCost { get; private set; }

    public static Result<Movie> Create(
        string code,
        string title,
        string director,
        int year,
        int durationMinutes,
        IEnumerable<Genre> genres,
        string language,
        decimal rating,
        int seasons,
        decimal rentalCost)
    {
        var genreList = genres?.Distinct().ToList() ?? [];

        var error = ValidateCode(code)
            ?? ValidateTitle(title)
            ?? ValidateDirector(director)
            ?? ValidateYear(year)
            ?? ValidateDuration(durationMinutes)
            ?? ValidateGenres(genreList)
            ?? ValidateLanguage(language)
            ?? ValidateRating(rating)
            ?? ValidateSeasons(seasons)
            ?? ValidateRentalCost(rentalCost);

        if (error is not null)
            return Result<Movie>.Failure(error);

        var movie = new Movie
        {
            Code = code,
            Title = title.Trim(),
            Director = director.Trim(),
            Year = year,
            DurationMinutes = durationMinutes,
            Language = language.Trim(),
            Rating = rating,
            Seasons = seasons,
            RentalCost = rentalCost,
        };
        movie._genres.AddRange(genreList);

        return Result<Movie>.Success(movie);
    }

    public Result Update(
        int durationMinutes,
        IEnumerable<Genre> genres,
        string language,
        decimal rating,
        int seasons,
        decimal rentalCost)
    {
        var genreList = genres?.Distinct().ToList() ?? [];

        var error = ValidateDuration(durationMinutes)
            ?? ValidateGenres(genreList)
            ?? ValidateLanguage(language)
            ?? ValidateRating(rating)
            ?? ValidateSeasons(seasons)
            ?? ValidateRentalCost(rentalCost);

        if (error is not null)
            return Result.Failure(error);

        DurationMinutes = durationMinutes;
        _genres.Clear();
        _genres.AddRange(genreList);
        Language = language.Trim();
        Rating = rating;
        Seasons = seasons;
        RentalCost = rentalCost;

        return Result.Success();
    }

    private static string? ValidateCode(string code) =>
        !string.IsNullOrEmpty(code) && CodePattern.IsMatch(code)
            ? null
            : "Code must be exactly 10 alphanumeric characters.";

    private static string? ValidateTitle(string title) =>
        !string.IsNullOrEmpty(title) && TitlePattern.IsMatch(title.Trim())
            ? null
            : "Title must be at least 2 characters and contain only letters, digits, spaces or dashes.";

    private static string? ValidateDirector(string director)
    {
        var trimmed = director?.Trim() ?? string.Empty;

        if (trimmed.Length == 0 || !DirectorPattern.IsMatch(trimmed))
            return "Director must contain only letters and spaces.";

        return trimmed.Count(char.IsLetter) < 2
            ? "Director must contain at least 2 letters."
            : null;
    }

    private static string? ValidateYear(int year) =>
        year >= 1888 && year <= DateTime.UtcNow.Year
            ? null
            : $"Year must be between 1888 and {DateTime.UtcNow.Year}.";

    private static string? ValidateDuration(int durationMinutes) =>
        durationMinutes > 0
            ? null
            : "Duration must be a positive number of minutes.";

    private static string? ValidateGenres(IReadOnlyCollection<Genre> genres)
    {
        if (genres.Count == 0)
            return "At least one genre is required.";

        return genres.Any(g => !Enum.IsDefined(g))
            ? "One or more genres are invalid."
            : null;
    }

    private static string? ValidateLanguage(string language) =>
        !string.IsNullOrEmpty(language) && LanguagePattern.IsMatch(language.Trim())
            ? null
            : "Language must be at least 2 characters and contain only letters.";

    private static string? ValidateRating(decimal rating) =>
        rating is >= 0.0m and <= 10.0m
            ? null
            : "Rating must be between 0.0 and 10.0.";

    private static string? ValidateSeasons(int seasons) =>
        seasons >= 0
            ? null
            : "Seasons must be zero or greater.";

    private static string? ValidateRentalCost(decimal rentalCost) =>
        rentalCost > 0
            ? null
            : "Rental cost must be a positive amount.";
}
