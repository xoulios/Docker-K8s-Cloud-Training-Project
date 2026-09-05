using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MovieStreaming.Infrastructure.Persistence;

public class MovieDbContextFactory : IDesignTimeDbContextFactory<MovieDbContext>
{
    public MovieDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MovieDb")
            ?? "Server=localhost,1433;Database=MovieStreamingDb;User Id=movieapi;Password=Movie_Api_2026!;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<MovieDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new MovieDbContext(optionsBuilder.Options);
    }
}
