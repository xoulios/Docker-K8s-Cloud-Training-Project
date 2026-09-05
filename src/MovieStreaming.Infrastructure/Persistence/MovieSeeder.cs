using Microsoft.EntityFrameworkCore;
using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Infrastructure.Persistence;

public static class MovieSeeder
{
    public static async Task SeedAsync(MovieDbContext dbContext)
    {
        await dbContext.Database.MigrateAsync();

        if (await dbContext.Movies.AnyAsync())
            return;

        var seedMovies = new[]
        {
            Movie.Create("MX00000001", "The Matrix", "Wachowski Sisters", 1999, 136,
                [Genre.SciFi, Genre.Action], "English", 8.7m, 0, 3.99m),
            Movie.Create("BR00000002", "Breaking Bad", "Vince Gilligan", 2008, 47,
                [Genre.Drama, Genre.Thriller], "English", 9.5m, 5, 2.49m),
            Movie.Create("PS00000003", "Parasite", "Bong Joon Ho", 2019, 132,
                [Genre.Drama, Genre.Thriller], "Korean", 8.6m, 0, 4.49m),
        };

        foreach (var result in seedMovies)
            dbContext.Movies.Add(result.Value!);

        await dbContext.SaveChangesAsync();
    }
}
