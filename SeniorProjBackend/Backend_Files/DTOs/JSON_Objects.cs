using System.Text.Json.Serialization;

namespace SeniorProjBackend.DTOs
{
    public class SubmissionResult
    {
        [JsonPropertyName("language_id")]
        public int LanguageId { get; set; }

        [JsonPropertyName("stdout")]
        public string? Stdout { get; set; }

        [JsonPropertyName("stderr")]
        public string? Stderr { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("compile_output")]
        public string? CompileOutput { get; set; }

        [JsonPropertyName("status")]
        public StatusResult Status { get; set; }

        [JsonPropertyName("language")]
        public LanguageResult Language { get; set; }

        [JsonPropertyName("time")]
        public float? Time { get; set; }

        [JsonPropertyName("memory")]
        public float? Memory { get; set; }
    }

    public class StatusResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class LanguageResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
