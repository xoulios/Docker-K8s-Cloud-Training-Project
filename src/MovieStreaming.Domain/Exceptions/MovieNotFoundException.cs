namespace MovieStreaming.Domain.Exceptions;

public sealed class MovieNotFoundException(string code) : Exception($"No movie found with code '{code}'.");
