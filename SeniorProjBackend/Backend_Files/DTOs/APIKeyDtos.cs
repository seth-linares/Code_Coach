namespace SeniorProjBackend.DTOs
{
    public class CreateAPIKeyRequest
    {
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
    }

    public class UpdateAPIKeyRequest
    {
        public int APIKeyID { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
    }


    public class APIKeyListItemDto
    {
        public int APIKeyID { get; set; }
        public string KeyName { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUsedAt { get; set; }
        public int UsageCount { get; set; }
    }


    public class APIKeyUsageDto
    {
        public int APIKeyID { get; set; }
        public string KeyName { get; set; }
        public int UsageCount { get; set; }
        public int TotalTokensUsed { get; set; }
        public DateTimeOffset? LastUsedAt { get; set; }
        public double AverageTokensPerUse { get; set; }
    }
}
