namespace some.reads.tech.Helpers;

public static class PromptBuilder
{
    public static object Build(string prompt)
    {
        return new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };
    }
}