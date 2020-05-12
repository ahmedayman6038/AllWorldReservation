using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using AllWorldReservation.DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AllWorldReservation.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Property> Properties { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}