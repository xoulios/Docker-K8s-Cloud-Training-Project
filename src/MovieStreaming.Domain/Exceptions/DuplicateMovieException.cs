namespace MovieStreaming.Domain.Exceptions;

public sealed class DuplicateMovieException(string message) : Exception(message);
