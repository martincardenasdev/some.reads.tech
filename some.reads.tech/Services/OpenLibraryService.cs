namespace some.reads.tech.Services
{
    public sealed class OpenLibraryService(HttpClient httpClient)
    {
        public async Task<T?> GetFromJsonAsync<T>(string url) => await httpClient.GetFromJsonAsync<T>(url);
    }
}
