using System.Text.Json.Serialization;

namespace MovieStreaming.Domain.Enums;

public enum Genre
{
    Action,
    Comedy,
    Drama,
    Horror,
    Thriller,
    Romance,
    Animation,
    Documentary,
    [JsonStringEnumMemberName("Sci-fi")]
    SciFi,
    Fantasy,
}
