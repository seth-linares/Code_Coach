using Newtonsoft.Json;
using SeniorProjBackend.Data;
using System.Net.Http.Headers;
using System.Text;

namespace SeniorProjBackend.Middleware
{

    public interface IChatGPTService
    {
        Task<ChatGPTResponse> SendMessage(string apiKey, List<AIMessage> conversationHistory, string newMessage);
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

        public async Task<ChatGPTResponse> SendMessage(string apiKey, List<AIMessage> conversationHistory, string newMessage)
        {
            var messages = new List<object>
        {
            new { role = "system", content = AI_TUTOR_PROMPT }
        };

            messages.AddRange(conversationHistory.Select(m => new { role = m.Role, content = m.Content }));
            messages.Add(new { role = "user", content = newMessage });

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = messages
            };

            var request = new HttpRequestMessage(HttpMethod.Post, API_URL)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var chatGPTResponse = JsonConvert.DeserializeObject<ChatGPTResponse>(responseString);
                return chatGPTResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling ChatGPT API");
                throw;
            }
        }

        private const string AI_TUTOR_PROMPT = @"
        You are an AI tutor for a student learning computer science principles and fundamentals. 
        Your primary goal is to teach and explain computer science concepts clearly and accurately. 
        Focus on explaining theoretical concepts and principles rather than providing code solutions. 
        Only provide code examples if explicitly asked by the student. 
        Always encourage the student to think critically and solve problems on their own.";
    }

    public class ChatGPTResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public string SystemFingerprint { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
