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

            // Fluent API section, will need to add indexing later in development





            // AIConversation Table

            // Primary Key
            modelBuilder.Entity<AIConversation>()
                .HasKey(c => c.ConversationID);

            // One-to-many relationship between AIConversation and User
            modelBuilder.Entity<AIConversation>()
                .HasOne(c => c.User)
                .WithMany(u => u.AIConversations) // Users navigation property
                .HasForeignKey(c => c.UserID)
                .IsRequired() // Required to have a user
                .OnDelete(DeleteBehavior.Cascade); // Set Cascade for now but also could become ClientCascade 

            // One-to-many relationship between AIConversation and Problem
            modelBuilder.Entity<AIConversation>()
                .HasOne(c => c.Problem)
                .WithMany(p => p.AIConversations)  // Problems navigation property
                .HasForeignKey(c => c.ProblemID)
                .OnDelete(DeleteBehavior.SetNull); // Set null for now, but could change to ClientSetNull for more fine control 

            // Properties
            modelBuilder.Entity<AIConversation>()
                .Property(c => c.ConversationID)
                .ValueGeneratedOnAdd(); // should auto increment 
            modelBuilder.Entity<AIConversation>()
                .Property(c => c.Timestamp)
                .HasColumnType("datetime2") // should be datetime2
                .HasDefaultValueSql("GETDATE()") // should default to current date and time. Will need to modify timestamp each time we change content
                .IsRequired();
            modelBuilder.Entity<AIConversation>()
                .Property(c => c.ConversationContent)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            modelBuilder.Entity<AIConversation>()
                .Property(c => c.IsCompleted)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .IsRequired();









            // APIKey Table
            modelBuilder.Entity<APIKey>()
                .HasKey(a => a.APIKeyID);

            // One-to-many relationship between APIKey and User
            modelBuilder.Entity<APIKey>()
                .HasOne(a => a.User)
                .WithMany(u => u.APIKeys)
                .HasForeignKey(a => a.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Properties
            modelBuilder.Entity<APIKey>()
                .Property(a => a.APIKeyID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<APIKey>()
                .Property(a => a.KeyType)
                .HasColumnType("varchar(50)")
                .IsRequired();
            modelBuilder.Entity<APIKey>()
                .Property(a => a.KeyValue)
                .HasColumnType("varchar(255)")
                .IsRequired();
            modelBuilder.Entity<APIKey>()
                .Property(a => a.Permissions)
                .HasColumnType("varchar(255)");
            modelBuilder.Entity<APIKey>()
                .Property(a => a.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
            modelBuilder.Entity<APIKey>()
                .Property(a => a.ExpiresAt)
                .HasColumnType("datetime2");




            // AuditLog Table
            modelBuilder.Entity<AuditLog>()
                .HasKey(AuditLog => AuditLog.AuditLogID);

            // One-to-many relationship between AuditLog and User
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            // Properties
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.AuditLogID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.EventType) // We have an enum for this!! We convert it to a string for storage
                .HasConversion<string>()
                .HasColumnType("varchar(50)")
                .IsRequired();
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Details)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Timestamp)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();


            // Category Table
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryID);

            //// handle many-to-many relationship between Category and Problem
            //modelBuilder.Entity<Category>()
            //    .HasMany(c => c.ProblemCategories)
            //    .WithOne(pc => pc.Category)
            //    .HasForeignKey(pc => pc.CategoryID)
            //    .OnDelete(DeleteBehavior.Cascade); // Set Cascade for now

            // Properties
            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .HasColumnType("varchar(50)")
                .IsRequired();



            // Feedback Table
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedbackID);

            // One-to-many relationship between Feedback and User
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Feedback and Problem
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Problem)
                .WithMany(p => p.Feedbacks)
                .HasForeignKey(f => f.ProblemID)
                .OnDelete(DeleteBehavior.Cascade);

            // Properties
            modelBuilder.Entity<Feedback>()
                .Property(f => f.FeedbackID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Feedback>()
                .Property(f => f.FeedbackText)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            modelBuilder.Entity<Feedback>()
                .Property(f => f.SubmissionTime)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();


        }

    }
}
