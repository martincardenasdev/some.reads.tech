using some.reads.tech.Filters;
using some.reads.tech.Services;

namespace some.reads.tech.Features.AI;
    
public static class BookSummary
{
    public static void AddBookSummaryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("books/summary", Handler).AddEndpointFilter<CacheEndpointFilter>();
    }

    private static async Task<IResult> Handler(string bookName, GeminiService geminiService)
    {
        var prompt = $"Summarize the book '{bookName}' in a concise manner.";
        
        var body = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };

        var response = await geminiService.GenerateContentAsync<object>("v1beta/models/gemini-2.0-flash:generateContent", body);
        
        return response is null ? Results.NotFound(new { message = "No summary found" }) : Results.Ok(new { summary = response });
    }
}