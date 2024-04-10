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
                .OnDelete(DeleteBehavior.SetNull); // If a User is deleted, the UserID in the AuditLog table is set to null

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

            // handle one-to-many relationship between Category and ProblemCategory
            modelBuilder.Entity<Category>()
                .HasMany(c => c.ProblemCategories)
                .WithOne(pc => pc.Category)
                .HasForeignKey(pc => pc.CategoryID)
                .OnDelete(DeleteBehavior.Cascade); // if a Category is deleted, all associated ProblemCategories are also deleted

            // Properties
            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .HasColumnType("varchar(50)")
                .IsRequired();








            // Feedback Table
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedbackID);

            /*
             * THE CASCADE DELETING MIGHT HAVE TO BE CHANGED TO CLIENTSETNULL FOR PROBLEM
             * IT IS UNCLEAR UNTIL WE HAVE SOME TESTING DONE
             * FOR NOW ASSUME IT IS FINE BUT IT MIGHT NEED TO BE CHANGED
            */ 

            // One-to-many relationship between Feedback and User
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // if a User is deleted, all associated Feedbacks are also deleted

            // One-to-many relationship between Feedback and Problem
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Problem)
                .WithMany(p => p.Feedbacks)
                .HasForeignKey(f => f.ProblemID)
                .IsRequired(false) // Ensure ProblemID is nullable
                .OnDelete(DeleteBehavior.ClientSetNull); // Set ProblemID to null in Feedback when a Problem is deleted


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








            // Language Table
            modelBuilder.Entity<Language>()
                .HasKey(l => l.LanguageID);

            // one-to-many relationship between Language and ProblemLanguage
            modelBuilder.Entity<Language>()
                .HasMany(l => l.ProblemLanguages)
                .WithOne(pl => pl.Language)
                .HasForeignKey(pl => pl.LanguageID)
                .OnDelete(DeleteBehavior.Cascade); // if a Language is deleted, all associated ProblemLanguages are also deleted
            
            // Properties
            modelBuilder.Entity<Language>()
                .Property(l => l.LanguageID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Language>()
                .Property(l => l.LanguageName)
                .HasColumnType("varchar(50)")
                .IsRequired();









            // Problem Table
            modelBuilder.Entity<Problem>()
                .HasKey(p => p.ProblemID);

            // One-to-many relationship between Problem and AIConversation
            modelBuilder.Entity<Problem>()
                .HasMany(p => p.AIConversations) // Each Problem can have many AIConversations
                .WithOne(ai => ai.Problem) // Each AIConversation is associated with one Problem
                .HasForeignKey(ai => ai.ProblemID) // The foreign key in the AIConversation table is ProblemID
                .OnDelete(DeleteBehavior.SetNull); // If a Problem is deleted, the ProblemID in the AIConversation table is set to null


            // One-to-many relationship between Problem and Feedback
            modelBuilder.Entity<Problem>()
                .HasMany(p => p.Feedbacks) 
                .WithOne(f => f.Problem) 
                .HasForeignKey(f => f.ProblemID) 
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated Feedbacks are also deleted

            // One-to-many relationship between Problem and ProblemCategory
            modelBuilder.Entity<Problem>()
                .HasMany(p => p.ProblemCategories) 
                .WithOne(pc => pc.Problem) 
                .HasForeignKey(pc => pc.ProblemID) 
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated ProblemCategories are also deleted

            // One-to-many relationship between Problem and ProblemLanguage
            modelBuilder.Entity<Problem>()
                .HasMany(p => p.ProblemLanguages) 
                .WithOne(pl => pl.Problem) 
                .HasForeignKey(pl => pl.ProblemID) 
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated ProblemLanguages are also deleted

            // One-to-many relationship between Problem and UserSubmission
            modelBuilder.Entity<Problem>()
                .HasMany(p => p.UserSubmissions) 
                .WithOne(us => us.Problem) 
                .HasForeignKey(us => us.ProblemID) 
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated UserSubmissions are also deleted


            // Properties
            modelBuilder.Entity<Problem>()
                .Property(p => p.ProblemID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Problem>()
                .Property(p => p.Title)
                .HasColumnType("nvarchar(250)")
                .IsRequired();
            modelBuilder.Entity<Problem>()
                .Property(p => p.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            modelBuilder.Entity<Problem>()
                .Property(p => p.DifficultyScore)
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Problem>()
                .Property(p => p.IsActive)
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();
            modelBuilder.Entity<Problem>()
                .Property(p => p.LastModifiedDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
            modelBuilder.Entity<Problem>()
                .Property(p => p.TestCodeFileName)
                .HasColumnType("nvarchar(250)")
                .IsRequired();
            






            // ProblemCategory Table
            modelBuilder.Entity<ProblemCategory>()
                .HasKey(pc => pc.ProblemCategoryID);

            // One-to-many relationship between ProblemCategory and Problem
            modelBuilder.Entity<ProblemCategory>()
                .HasOne(pc => pc.Problem)
                .WithMany(p => p.ProblemCategories)
                .HasForeignKey(pc => pc.ProblemID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated ProblemCategories are also deleted

            // One-to-many relationship between ProblemCategory and Category
            modelBuilder.Entity<ProblemCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProblemCategories)
                .HasForeignKey(pc => pc.CategoryID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Category is deleted, all associated ProblemCategories are also deleted

            // Property
            modelBuilder.Entity<ProblemCategory>()
                .Property(pc => pc.ProblemCategoryID)
                .ValueGeneratedOnAdd();





            // ProblemLanguage Table
            modelBuilder.Entity<ProblemLanguage>()
                .HasKey(pl => pl.ProblemLanguageID);

            // One-to-many relationship between ProblemLanguage and Problem
            modelBuilder.Entity<ProblemLanguage>()
                .HasOne(pl => pl.Problem)
                .WithMany(p => p.ProblemLanguages)
                .HasForeignKey(pl => pl.ProblemID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated ProblemLanguages are also deleted


            // One-to-many relationship between ProblemLanguage and Language
            modelBuilder.Entity<ProblemLanguage>()
                .HasOne(pl => pl.Language)
                .WithMany(l => l.ProblemLanguages)
                .HasForeignKey(pl => pl.LanguageID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Language is deleted, all associated ProblemLanguages are also deleted

            // Property
            modelBuilder.Entity<ProblemLanguage>()
                .Property(pl => pl.ProblemLanguageID)
                .ValueGeneratedOnAdd();





            // RecoveryCode Table
            modelBuilder.Entity<RecoveryCode>()
                .HasKey(rc => rc.RecoveryCodeID);

            // One-to-many relationship between RecoveryCode and User
            modelBuilder.Entity<RecoveryCode>()
                .HasOne(rc => rc.User)
                .WithMany(u => u.RecoveryCodes)
                .HasForeignKey(rc => rc.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated RecoveryCodes are also deleted

            // Properties
            modelBuilder.Entity<RecoveryCode>()
                .Property(rc => rc.RecoveryCodeID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<RecoveryCode>()
                .Property(rc => rc.Code)
                .HasColumnType("varchar(255)") // Hashed recovery code
                .IsRequired();
            modelBuilder.Entity<RecoveryCode>()
                .Property(rc => rc.CreationDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();




            // User Table
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            // One-to-many relationship between User and AIConversation
            modelBuilder.Entity<User>()
                .HasMany(u => u.AIConversations)
                .WithOne(ai => ai.User)
                .HasForeignKey(ai => ai.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated AIConversations are also deleted

            // One-to-many relationship between User and APIKey
            modelBuilder.Entity<User>()
                .HasMany(u => u.APIKeys)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated APIKeys are also deleted

            // One-to-many relationship between User and Feedback
            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated Feedbacks are also deleted

            // One-to-many relationship between User and RecoveryCode
            modelBuilder.Entity<User>()
                .HasMany(u => u.RecoveryCodes)
                .WithOne(rc => rc.User)
                .HasForeignKey(rc => rc.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated RecoveryCodes are also deleted

            // One-to-many relationship between User and UserPreference
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserPreferences)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated UserPreferences are also deleted

            // One-to-many relationship between User and AuditLog
            modelBuilder.Entity<User>()
                .HasMany(u => u.AuditLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserID)
                .OnDelete(DeleteBehavior.SetNull); // If a User is deleted, the UserID in the AuditLog table is set to null

            // One-to-many relationship between User and UserSubmission
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSubmissions)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated UserSubmissions are also deleted

            // Properties
            modelBuilder.Entity<User>()
                .Property(u => u.UserID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasColumnType("varchar(50)")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .HasColumnType("varchar(255)")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.EmailAddress)
                .HasColumnType("varchar(255)")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.TwoFactorEnabled)
                .HasColumnType("bit")
                .HasDefaultValue(false)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.SecretKey)
                .HasColumnType("varchar(255)"); // encrypted secret key for 2FA; nullable
            modelBuilder.Entity<User>()
                .Property(u => u.TotalScore)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Bio)
                .HasColumnType("nvarchar(max)"); // might remove since we likely won't do community features
            modelBuilder.Entity<User>()
                .Property(u => u.ProfilePictureURL)
                .HasColumnType("varchar(255)")
                .HasDefaultValue("path/to/default/profile-picture.jpg")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.RegistrationDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.LastActiveDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Rank)
                .HasColumnType("varchar(50)")
                .HasDefaultValue("Newbie")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.RankIconURL)
                .HasColumnType("varchar(255)")
                .HasDefaultValue("path/to/default/rank-icon.jpg")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.ActiveStreak)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            // Unique constraints and indexes

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();



            
            // UserPreference Table
            modelBuilder.Entity<UserPreference>()
                .HasKey(up => up.UserPreferenceID);

            // One-to-many relationship between UserPreference and User
            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPreferences)
                .HasForeignKey(up => up.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated UserPreferences are also deleted

            // Properties
            modelBuilder.Entity<UserPreference>()
                .Property(up => up.UserPreferenceID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<UserPreference>()
                .Property(up => up.PreferenceKey)
                .HasConversion<string>() // Convert enum to string for storage
                .HasColumnType("varchar(50)")
                .IsRequired();
            modelBuilder.Entity<UserPreference>()
                .Property(up => up.PreferenceValue)
                .HasColumnType("varchar(255)")
                .IsRequired();



            // UserSubmission Table
            modelBuilder.Entity<UserSubmission>()
                .HasKey(us => us.SubmissionID);

            // One-to-many relationship between UserSubmission and User
            modelBuilder.Entity<UserSubmission>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSubmissions)
                .HasForeignKey(us => us.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a User is deleted, all associated UserSubmissions are also deleted

            // One-to-many relationship between UserSubmission and Problem
            modelBuilder.Entity<UserSubmission>()
                .HasOne(us => us.Problem)
                .WithMany(p => p.UserSubmissions)
                .HasForeignKey(us => us.ProblemID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Problem is deleted, all associated UserSubmissions are also deleted

            // One-to-many relationship between UserSubmission and Language
            modelBuilder.Entity<UserSubmission>()
                .HasOne(us => us.Language)
                .WithMany(l => l.UserSubmissions)
                .HasForeignKey(us => us.LanguageID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a Language is deleted, all associated UserSubmissions are also deleted

            // Properties
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.SubmissionID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.SubmittedCode)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.SubmissionTime)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.IsSuccessful)
                .HasColumnType("bit")
                .IsRequired();
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.ScoreAwarded) // This might belong in the Problems table, not sure!!!
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.ExecutionTime)
                .HasColumnType("int");
            modelBuilder.Entity<UserSubmission>()
                .Property(us => us.MemoryUsage)
                .HasColumnType("int");

        }

    }
}
