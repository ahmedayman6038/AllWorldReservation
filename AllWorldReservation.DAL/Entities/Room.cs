using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public float PriceUSD { get; set; }

        public float PriceEGP { get; set; }

        public int? HotelId { get; set; }

        public int Guests { get; set; }
        
        public virtual Hotel Hotel { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
