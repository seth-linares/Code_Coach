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

                _logger.LogInformation("\n\n\n\n\nRaw API Response:\n{responseString}\n\n\n\n\n", responseString);

                var chatGPTResponse = JsonSerializer.Deserialize<ChatGPTResponse>(responseString, options);

                if (chatGPTResponse != null && chatGPTResponse.Usage != null)
                {
                    _logger.LogInformation("\n\n\n\nCompletion Tokens: {CompletionTokens}\nPrompt Tokens: {PromptTokens}\nTotal Tokens: {TotalTokens}\n\n\n\n",
                        chatGPTResponse.Usage.CompletionTokens, chatGPTResponse.Usage.PromptTokens, chatGPTResponse.Usage.TotalTokens);
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
        You are an AI computer science tutor. Provide clear, comprehensive explanations of CS concepts, emphasizing:

        1. Detailed theoretical explanations
        2. Code examples only when explicitly requested
        3. Critical thinking and problem-solving encouragement
        4. Adaptive teaching based on student's level
        5. Relevant analogies and real-world examples
        6. 1-2 follow-up questions to deepen understanding
        7. Consideration of provided code problem context
        8. Balance between assistance and fostering independence
        9. Multiple approaches or interpretations when applicable
        10. Step-by-step explanations for complex topics

        Focus on education and guidance, not direct problem-solving.";
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