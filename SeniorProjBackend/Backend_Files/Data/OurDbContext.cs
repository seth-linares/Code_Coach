using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace SeniorProjBackend.Data
{
    public class OurDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        // DbSet for each table

        public DbSet<AIConversation> AIConversations { get; set; }
        public DbSet<APIKey> APIKeys { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProblemLanguage> ProblemLanguages { get; set; }
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


            // You may want to customize the names of Identity tables
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            modelBuilder.Entity<User>(entity =>
            {

                entity.Property(e => e.ProfilePictureURL)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValue("https://cdn.pfps.gg/pfps/9150-cat-25.png");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TotalScore)
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                entity.Property(e => e.Rank)
                    .HasColumnType("integer")
                    .HasComputedColumnSql("CASE " +
                        "WHEN \"TotalScore\" >= 300 THEN 4 " +
                        "WHEN \"TotalScore\" >= 150 THEN 3 " +
                        "WHEN \"TotalScore\" >= 75 THEN 2 " +
                        "WHEN \"TotalScore\" >= 25 THEN 1 " +
                        "ELSE 0 END", stored: true);

                entity.Property(e => e.CompletedProblems)
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                entity.Property(e => e.AttemptedProblems)
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                entity.HasMany(e => e.AIConversations)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.APIKeys)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserSubmissions)
                    .WithOne()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });




            modelBuilder.Entity<AIConversation>(entity =>
            {
                entity.HasKey(e => e.ConversationID);

                entity.Property(e => e.ConversationID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.UserId)
                    .HasColumnType("integer");

                entity.Property(e => e.ProblemID)
                    .HasColumnType("integer");


                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Model)
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("'gpt-4o-mini'");

                entity.Property(e => e.TotalTokens)
                    .HasColumnType("integer");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.AIConversations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Problem)
                    .WithMany()
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.SetNull);


                entity.HasMany(e => e.Messages)
                    .WithOne(e => e.Conversation)
                    .HasForeignKey(e => e.ConversationID)
                    .OnDelete(DeleteBehavior.Cascade);
            });




            modelBuilder.Entity<AIMessage>(entity =>
            {
                entity.HasKey(e => e.MessageID);

                entity.Property(e => e.MessageID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.ConversationID)
                    .HasColumnType("integer");

                entity.Property(e => e.Content)
                    .HasColumnType("text");

                entity.Property(e => e.Role)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValue("assistant");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.PromptTokens)
                    .HasColumnType("integer");

                entity.Property(e => e.CompletionTokens)
                    .HasColumnType("integer");

                entity.HasOne(e => e.Conversation)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.ConversationID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<APIKey>(entity =>
            {
                entity.HasKey(e => e.APIKeyID);

                entity.Property(e => e.APIKeyID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.UserId)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.KeyName)
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                entity.Property(e => e.KeyValue)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasColumnType("boolean")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                entity.Property(e => e.LastUsedAt)
                    .HasColumnType("timestamptz");

                entity.Property(e => e.UsageCount)
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.APIKeys)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });




            modelBuilder.Entity<Problem>(entity =>
            {
                entity.HasKey(e => e.ProblemID);

                entity.Property(e => e.ProblemID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(e => e.Points)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.Difficulty)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.Category)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.HasMany(e => e.ProblemLanguages)
                    .WithOne(e => e.Problem)
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserSubmissions)
                    .WithOne(e => e.Problem)
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AIConversations)
                    .WithOne(e => e.Problem)
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.SetNull);
            });



            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.LanguageID);

                entity.Property(e => e.LanguageID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Judge0ID)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.HasMany(e => e.ProblemLanguages)
                    .WithOne(e => e.Language)
                    .HasForeignKey(e => e.LanguageID)
                    .OnDelete(DeleteBehavior.Cascade);
            });





            modelBuilder.Entity<ProblemLanguage>(entity =>
            {
                entity.HasKey(e => e.ProblemLanguageID);

                entity.Property(e => e.ProblemLanguageID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.ProblemID)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.LanguageID)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.FunctionSignature)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(e => e.TestCode)
                    .HasColumnType("text")
                    .IsRequired();

                entity.HasOne(e => e.Problem)
                    .WithMany(e => e.ProblemLanguages)
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Language)
                    .WithMany(e => e.ProblemLanguages)
                    .HasForeignKey(e => e.LanguageID)
                    .OnDelete(DeleteBehavior.Cascade);
            });





            modelBuilder.Entity<UserSubmission>(entity =>
            {
                entity.HasKey(e => e.SubmissionID);

                entity.Property(e => e.SubmissionID)
                    .HasColumnType("integer")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.UserId)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.ProblemID)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.LanguageID)
                    .HasColumnType("integer")
                    .IsRequired();

                entity.Property(e => e.SubmittedCode)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                entity.Property(e => e.IsSuccessful)
                    .HasColumnType("boolean")
                    .IsRequired();

                entity.Property(e => e.Token)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.ExecutionTime)
                    .HasColumnType("real");

                entity.Property(e => e.MemoryUsed)
                    .HasColumnType("real");

                // Navigation properties
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserSubmissions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Problem)
                    .WithMany(p => p.UserSubmissions)
                    .HasForeignKey(e => e.ProblemID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Language)
                    .WithMany()
                    .HasForeignKey(e => e.LanguageID)
                    .OnDelete(DeleteBehavior.Cascade);
            });





        }

    }
}
