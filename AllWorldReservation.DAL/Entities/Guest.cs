using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(5)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }

        public int Type { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int? ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}
