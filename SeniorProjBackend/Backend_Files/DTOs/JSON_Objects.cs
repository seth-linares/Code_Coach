using Newtonsoft.Json;

namespace SeniorProjBackend.DTOs
{
    public class SubmissionResult
    {
        [JsonProperty("language_id")]
        public int LanguageId { get; set; }

        [JsonProperty("stdout")]
        public string? Stdout { get; set; }

        [JsonProperty("time")]
        public float? Time { get; set; }

        [JsonProperty("memory")]
        public float? Memory { get; set; }
        
        [JsonProperty("stderr")]
        public string? Stderr { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("compile_output")]
        public string? CompileOutput { get; set; }

        [JsonProperty("status")]
        public StatusResult Status { get; set; }
    }

    public class StatusResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
