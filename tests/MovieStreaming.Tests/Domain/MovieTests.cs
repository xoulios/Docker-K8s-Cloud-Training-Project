using MovieStreaming.Domain.Common;
using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Tests.Domain;

public class MovieTests
{
    private static Result<Movie> CreateValid(
        string code = "AB12345678",
        string title = "The Matrix",
        string director = "Wachowski Sisters",
        int year = 1999,
        int duration = 136,
        Genre[]? genres = null,
        string language = "English",
        decimal rating = 8.7m,
        int seasons = 0,
        decimal rentalCost = 3.99m)
        => Movie.Create(code, title, director, year, duration,
            genres ?? [Genre.SciFi, Genre.Action], language, rating, seasons, rentalCost);

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal("AB12345678", result.Value!.Code);
        Assert.Equal(2, result.Value.Genres.Count);
    }

    [Theory]
    [InlineData("SHORT")]
    [InlineData("TOOLONGCODE1")]
    [InlineData("HAS-DASH12")]
    [InlineData("")]
    public void Create_WithInvalidCode_Fails(string code)
    {
        var result = CreateValid(code: code);

        Assert.True(result.IsFailure);
        Assert.Contains("Code", result.Error);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("")]
    [InlineData("Bad_Title!")]
    public void Create_WithInvalidTitle_Fails(string title)
    {
        var result = CreateValid(title: title);

        Assert.True(result.IsFailure);
        Assert.Contains("Title", result.Error);
    }

    [Theory]
    [InlineData("A ")]
    [InlineData("X")]
    [InlineData("John123")]
    public void Create_WithInvalidDirector_Fails(string director)
    {
        var result = CreateValid(director: director);

        Assert.True(result.IsFailure);
        Assert.Contains("Director", result.Error);
    }

    [Theory]
    [InlineData("Al Smith")]
    [InlineData("Γιώργος Λάνθιμος")]
    public void Create_WithValidDirector_Succeeds(string director)
    {
        var result = CreateValid(director: director);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithYearBeforeCinemaBegan_Fails()
    {
        var result = CreateValid(year: 1887);

        Assert.True(result.IsFailure);
        Assert.Contains("Year", result.Error);
    }

    [Fact]
    public void Create_WithYearInTheFuture_Fails()
    {
        var result = CreateValid(year: DateTime.UtcNow.Year + 1);

        Assert.True(result.IsFailure);
        Assert.Contains("Year", result.Error);
    }

    [Fact]
    public void Create_WithCurrentYear_Succeeds()
    {
        var result = CreateValid(year: DateTime.UtcNow.Year);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_WithNonPositiveDuration_Fails(int duration)
    {
        var result = CreateValid(duration: duration);

        Assert.True(result.IsFailure);
        Assert.Contains("Duration", result.Error);
    }

    [Fact]
    public void Create_WithNoGenres_Fails()
    {
        var result = CreateValid(genres: []);

        Assert.True(result.IsFailure);
        Assert.Contains("genre", result.Error);
    }

    [Fact]
    public void Create_WithUndefinedGenre_Fails()
    {
        var result = CreateValid(genres: [(Genre)99]);

        Assert.True(result.IsFailure);
        Assert.Contains("genres are invalid", result.Error);
    }

    [Fact]
    public void Create_WithDuplicateGenres_KeepsDistinctValues()
    {
        var result = CreateValid(genres: [Genre.Drama, Genre.Drama, Genre.Action]);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Genres.Count);
    }

    [Theory]
    [InlineData("E")]
    [InlineData("En9lish")]
    [InlineData("")]
    public void Create_WithInvalidLanguage_Fails(string language)
    {
        var result = CreateValid(language: language);

        Assert.True(result.IsFailure);
        Assert.Contains("Language", result.Error);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(10.0)]
    [InlineData(5.5)]
    public void Create_WithRatingInsideRange_Succeeds(double rating)
    {
        var result = CreateValid(rating: (decimal)rating);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(10.1)]
    public void Create_WithRatingOutOfRange_Fails(double rating)
    {
        var result = CreateValid(rating: (decimal)rating);

        Assert.True(result.IsFailure);
        Assert.Contains("Rating", result.Error);
    }

    [Fact]
    public void Create_WithNegativeSeasons_Fails()
    {
        var result = CreateValid(seasons: -1);

        Assert.True(result.IsFailure);
        Assert.Contains("Seasons", result.Error);
    }

    [Fact]
    public void Create_WithZeroSeasons_SucceedsForMovies()
    {
        var result = CreateValid(seasons: 0);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveRentalCost_Fails(int cost)
    {
        var result = CreateValid(rentalCost: cost);

        Assert.True(result.IsFailure);
        Assert.Contains("Rental cost", result.Error);
    }

    [Fact]
    public void Create_TrimsWhitespaceFromTextFields()
    {
        var result = CreateValid(title: "  The Matrix  ", director: "  Lana Wachowski  ", language: "  English  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("The Matrix", result.Value!.Title);
        Assert.Equal("Lana Wachowski", result.Value.Director);
        Assert.Equal("English", result.Value.Language);
    }

    [Fact]
    public void Create_WithMultipleInvalidFields_FailsFastOnFirstError()
    {
        var result = CreateValid(code: "BAD", duration: -1, rating: 99m);

        Assert.True(result.IsFailure);
        Assert.Contains("Code", result.Error);
    }

    [Fact]
    public void Update_WithValidData_ChangesMutableFieldsOnly()
    {
        var movie = CreateValid().Value!;

        var result = movie.Update(200, [Genre.Drama], "French", 5.0m, 3, 9.99m);

        Assert.True(result.IsSuccess);
        Assert.Equal(200, movie.DurationMinutes);
        Assert.Equal("French", movie.Language);
        Assert.Equal(5.0m, movie.Rating);
        Assert.Equal(3, movie.Seasons);
        Assert.Equal(9.99m, movie.RentalCost);
        Assert.Single(movie.Genres);
        Assert.Equal("AB12345678", movie.Code);
        Assert.Equal("The Matrix", movie.Title);
        Assert.Equal("Wachowski Sisters", movie.Director);
        Assert.Equal(1999, movie.Year);
    }

    [Fact]
    public void Update_WithInvalidData_FailsAndLeavesStateUnchanged()
    {
        var movie = CreateValid().Value!;

        var result = movie.Update(-1, [], "F", 20.0m, -5, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(136, movie.DurationMinutes);
        Assert.Equal("English", movie.Language);
        Assert.Equal(8.7m, movie.Rating);
        Assert.Equal(2, movie.Genres.Count);
    }
}
