using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieStreaming.Domain.Entities;
using MovieStreaming.Domain.Enums;

namespace MovieStreaming.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Code);
        builder.Property(m => m.Code).HasMaxLength(10);
        builder.Property(m => m.Title).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Director).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Language).HasMaxLength(50).IsRequired();
        builder.Property(m => m.Rating).HasColumnType("decimal(3,1)");
        builder.Property(m => m.RentalCost).HasColumnType("decimal(8,2)");

        builder.HasIndex(m => new { m.Title, m.Year, m.Director }).IsUnique();

        builder.Ignore(m => m.Genres);
        builder.Property<List<Genre>>("_genres")
            .HasField("_genres")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                stored => string.Join(',', stored),
                raw => raw.Length == 0
                    ? new List<Genre>()
                    : raw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<Genre>).ToList(),
                new ValueComparer<List<Genre>>(
                    (left, right) => left != null && right != null && left.SequenceEqual(right),
                    genres => genres.Aggregate(0, (hash, genre) => HashCode.Combine(hash, genre)),
                    genres => genres.ToList()))
            .HasColumnName("Genres")
            .HasMaxLength(200)
            .IsRequired();
    }
}
