using Microsoft.EntityFrameworkCore;
using LaCasa.Models;

namespace LaCasa.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
    }
}
