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

        [Required]
        [Display(Name = "Title")]
        [MaxLength(5, ErrorMessage = "Max Lenght 5 Character")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Surname")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Type")]
        public GuestType Type { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Reservation")]
        public int? ReservationId { get; set; }

        [Display(Name = "Reservation")]
        public ReservationModel Reservation { get; set; }
    }
}
