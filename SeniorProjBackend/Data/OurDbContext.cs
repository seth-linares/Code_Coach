using Microsoft.EntityFrameworkCore;

namespace SeniorProjBackend.Data
{
    public class OurDbContext : DbContext 
    {
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options)
        {

        }
    }
}
