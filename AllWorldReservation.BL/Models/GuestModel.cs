using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class GuestModel
    {
        public int Id { get; set; }

        [MaxLength(5)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Type")]
        public GuestType Type { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Reservation")]
        public int? ReservationId { get; set; }

        [Display(Name = "Reservation")]
        public ReservationModel Reservation { get; set; }
    }
}
