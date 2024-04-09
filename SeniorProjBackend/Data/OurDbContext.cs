using Microsoft.EntityFrameworkCore;

namespace SeniorProjBackend.Data
{
    public class OurDbContext : DbContext 
    {
        // DbSet for each table
        
        public DbSet<AIConversation> AIConversations { get; set; }
        public DbSet<APIKey> APIKeys { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemCategory> ProblemCategories { get; set; }
        public DbSet<ProblemLanguage> ProblemLanguages { get; set; }
        public DbSet<RecoveryCode> RecoveryCodes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserSubmission> UserSubmissions { get; set; }

        /*
         * Constructor for OurDbContext. The options parameter is passed to the base class constructor.
         * The options parameter is an instance of DbContextOptions<OurDbContext>. 
         * This object is created by the dependency injection system and passed to the constructor.
        */
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API section


        }

    }
}
