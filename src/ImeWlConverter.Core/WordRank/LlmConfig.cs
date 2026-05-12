namespace ImeWlConverter.Core.WordRank;

/// <summary>
/// Configuration for LLM-based word rank generation.
/// </summary>
public sealed class LlmConfig
{
    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gpt-3.5-turbo";
}
