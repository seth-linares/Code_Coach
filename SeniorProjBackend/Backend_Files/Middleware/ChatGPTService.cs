using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeniorProjBackend.Middleware
{
    public interface IChatGPTService
    {
        Task<ChatGPTResponse> SendMessage(string apiKey, string userMessage);
    }

    public class ChatGPTService : IChatGPTService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatGPTService> _logger;
        private const string API_URL = "https://api.openai.com/v1/chat/completions";

        public ChatGPTService(HttpClient httpClient, ILogger<ChatGPTService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ChatGPTResponse> SendMessage(string apiKey, string userMessage)
        {
            var messages = new List<object>
            {
                new { role = "system", content = AI_TUTOR_PROMPT },
                new { role = "user", content = userMessage }
            };

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = messages
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var request = new HttpRequestMessage(HttpMethod.Post, API_URL)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody, options), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"\n\n\n\n\nRaw API Response:\n{responseString}\n\n\n\n\n");

                var chatGPTResponse = JsonSerializer.Deserialize<ChatGPTResponse>(responseString, options);

                if (chatGPTResponse != null && chatGPTResponse.Usage != null)
                {
                    _logger.LogInformation($"\n\n\n\nCompletion Tokens: {chatGPTResponse.Usage.CompletionTokens}\nPrompt Tokens: {chatGPTResponse.Usage.PromptTokens}\nTotal Tokens: {chatGPTResponse.Usage.TotalTokens}\n\n\n\n");
                }
                else
                {
                    _logger.LogWarning("\n\n\n\nUsage statistics not available in the response\n\n\n\n");
                }

                return chatGPTResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling ChatGPT API");
                throw;
            }
        }

        private const string AI_TUTOR_PROMPT = @"
    You are an AI tutor specializing in computer science principles and fundamentals. Your goal is to provide comprehensive, clear, and accurate explanations of computer science concepts. Key points:

    1. Offer detailed explanations of theoretical concepts and principles.
    2. Provide code examples only when explicitly requested by the student.
    3. Encourage critical thinking and problem-solving skills.
    4. Adapt your explanations to the student's level of understanding.
    5. When appropriate, use analogies or real-world examples to illustrate complex ideas.
    6. If a question is unclear, ask for clarification to ensure accurate responses.
    7. Offer 1-2 follow-up questions at the end of your response to deepen the student's understanding.
    8. If the student's question is in Base64, decode it before responding.
    9. Consider the code problem description (if provided) for context when answering.
    10. Maintain a balance between being helpful and encouraging independent learning.
    11. If a concept has multiple approaches or interpretations, mention this and explain the differences.
    12. Provide step-by-step explanations for complex processes or algorithms when relevant.

    Remember: Your primary role is to educate and guide, not to solve problems directly for the student.";
    }

    public class ChatGPTResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}