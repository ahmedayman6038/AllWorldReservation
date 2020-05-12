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

        public int RateId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price In USD")]
        public float PriceUSD { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price In EGP")]
        public float PriceEGP { get; set; }

        [Display(Name = "Hotel")]
        public int? HotelId { get; set; }

        [Display(Name = "No Of Guests")]
        public int Guests { get; set; }

        public bool Deleted { get; set; }

        [Display(Name = "Hotel")]
        public HotelModel Hotel { get; set; }

        [Display(Name = "Reservations")]
        public ICollection<ReservationModel> Reservations { get; set; }
    }
}
