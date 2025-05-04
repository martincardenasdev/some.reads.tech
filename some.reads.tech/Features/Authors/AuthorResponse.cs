using System.Text.Json.Serialization;

namespace some.reads.tech.Features.Authors
{
    public record AuthorResponse(
        string Name,
        string Key,
        [property: JsonPropertyName("alternate_names")] string[] AlternateNames,
        [property: JsonPropertyName("top_subjects")] string[] TopSubjects,
        [property: JsonPropertyName("top_work")] string TopWork,
        [property: JsonPropertyName("birth_date")] string BirthDate
        );
}
