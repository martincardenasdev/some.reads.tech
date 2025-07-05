using System.Text.Json.Serialization;

namespace some.reads.tech.Common
{
    public record OpenLibraryResponse<T>(
        int NumFound,
        [property: JsonPropertyName("q")] string Query,
        T[] Docs
    );
}
