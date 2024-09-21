using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SEM.Prototype.Models
{
    // Extend IdentityDbContext to include Identity functionality
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Keep your DbSet for EventModel or any other models

        public DbSet<EventModel> Events { get; set; }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<AvailableStaff> AvailableStaffs { get; set; }  // This is missing in your context
        public DbSet<AvailableSlot> AvailableSlots { get; set; }
        public DbSet<BookedAppointment> BookedAppointments { get; set; }
        public DbSet<UserAppointment> UserAppointments { get; set; }
    }
}
