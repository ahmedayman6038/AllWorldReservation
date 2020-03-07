using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class ReservationModel
    {
        public ReservationModel()
        {
            this.CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        //public int? UserId { get; set; }

        [Display(Name = "Payment Type")]
        public PayType PayType { get; set; }

        [Display(Name = "Reservation Type")]
        public ReservationType ReservationType { get; set; }

        [Display(Name = "Item Id")]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        [EmailAddress(ErrorMessage = "Not Valid Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telelphone No 1")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string TelNo1 { get; set; }

        [Display(Name = "Telelphone No 1")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string TelNo2 { get; set; }

        [Required]
        [Display(Name = "Address 1")]
        [MaxLength(150, ErrorMessage = "Max Lenght 50 Character")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        [MaxLength(150, ErrorMessage = "Max Lenght 50 Character")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        [MaxLength(10, ErrorMessage = "Max Lenght 50 Character")]
        public string PostCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Country { get; set; }

        [Display(Name = "Paid")]
        public bool Paied { get; set; }

        [Display(Name = "Order ID")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string OrderId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        //public virtual User User { get; set; }

        [Display(Name = "Guests")]
        public ICollection<GuestModel> Guests { get; set; }

        [Display(Name = "Rooms")]
        public ICollection<RoomModel> Rooms { get; set; }
    }
}
