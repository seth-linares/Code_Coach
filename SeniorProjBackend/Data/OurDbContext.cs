using Microsoft.EntityFrameworkCore;

namespace SeniorProjBackend.Data
{
    public class OurDbContext : DbContext 
    {
        // DbSet for each table
        public DbSet<User> Users { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<ProblemCategory> ProblemCategories { get; set; }
        public DbSet<ProblemLanguage> ProblemLanguages { get; set; }


        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options)
        {

        }
    }
}
