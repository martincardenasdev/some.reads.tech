namespace some.reads.tech.Services;

public sealed class GeminiService(HttpClient httpClient)
{
    public async Task<T?> GenerateContentAsync<T>(string url, object body)
    {
        var response = await httpClient.PostAsJsonAsync(url, body);
        
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<T>();
        
        throw new HttpRequestException($"Error generating content: {response.ReasonPhrase}");
    }
}