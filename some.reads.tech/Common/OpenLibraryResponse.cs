namespace some.reads.tech.Common
{
    public record OpenLibraryResponse<T>(
        int NumFound,
        T[] Docs
    );
}
