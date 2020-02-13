using AllWorldReservation.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Context
{
    public class DbContainer : DbContext
    {
        public DbContainer() : base("DbConnection"){ }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

    }
}
