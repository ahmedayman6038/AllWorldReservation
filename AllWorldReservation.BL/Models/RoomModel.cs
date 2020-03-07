using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class RoomModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "TotalAmount")]
        public float TotalAmount { get; set; }

        [Display(Name = "Hotel")]
        [Required(ErrorMessage = "Hotel Required")]
        public int? HotelId { get; set; }

        [Display(Name = "No Of Guests")]
        [Required(ErrorMessage = "No Of Guests Required")]
        public int Guests { get; set; }

        [Display(Name = "Hotel")]
        public HotelModel Hotel { get; set; }

        [Display(Name = "Reservations")]
        public ICollection<ReservationModel> Reservations { get; set; }
    }
}
