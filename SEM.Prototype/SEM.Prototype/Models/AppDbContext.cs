using Microsoft.EntityFrameworkCore;

namespace SEM.Prototype.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EventModel> Events { get; set; }  // Your DbSet for EventModel
    }
}
