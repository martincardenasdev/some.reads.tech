using System.Text.Json.Serialization;

namespace some.reads.tech.Features.Books.Get_Book
{
    public record   BookResponse(
        string? Title,
        BookAuthor[]? Authors,
        Description? Description,
        string[]? Subjects,
        [property: JsonPropertyName("subject_places")] string[]? SubjectPlaces,
        [property: JsonPropertyName("subject_people")] string[]? SubjectPeople,
        [property: JsonPropertyName("subject_times")] string[]? SubjectTimes,
        int[] Covers
        );

    public record BookAuthor(
    AuthorType Type,
    AuthorReference Author
    );

    public record AuthorType(
        string Key
        );

    public record AuthorReference(
        string Key
        );

    public record Description(
        string Type,
        string Value
        );
}
