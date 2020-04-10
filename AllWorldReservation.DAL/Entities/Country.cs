using AllWorldReservation.DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(2)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Place> Places { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
