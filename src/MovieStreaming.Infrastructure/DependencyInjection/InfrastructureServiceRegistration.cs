using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieStreaming.Domain.Services;
using MovieStreaming.Infrastructure.Persistence;
using MovieStreaming.Infrastructure.Services;

namespace MovieStreaming.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MovieDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
