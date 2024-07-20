namespace SeniorProjBackend.Data
{
    public class APIKey
    {
        public int APIKeyID { get; set; }
        public int UserId { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUsedAt { get; set; }
        public int UsageCount { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
    }
}
