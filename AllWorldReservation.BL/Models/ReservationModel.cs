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

        [Display(Name = "Payment Type")]
        public PayType PayType { get; set; }

        [Display(Name = "Reservation Type")]
        public ReservationType ReservationType { get; set; }

        [Display(Name = "Item Id")]
        public int ItemId { get; set; }

        [Display(Name = "Reserved Item")]
        public string ReservedItem { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Telelphone No 1")]
        public string TelNo1 { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telelphone No 2")]
        public string TelNo2 { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [MaxLength(150)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [MaxLength(10)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Paid")]
        public bool Paied { get; set; }

        [MaxLength(50)]
        [Display(Name = "Order ID")]
        public string OrderId { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total Amount")]
        public float TotalAmount { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public Currency Currency { get; set; }

        [Display(Name = "Reservation By")]
        public string UserId { get; set; }

        [Display(Name = "Approved")]
        public bool Approved { get; set; }

        [Required]
        [Display(Name = "Reservation From")]
        public DateTime ReservationFrom { get; set; }

        [Required]
        [Display(Name = "Reservation To")]
        public DateTime ReservationTo { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Reservation By")]
        public UserModel User { get; set; }

        [Display(Name = "Guests")]
        public ICollection<GuestModel> Guests { get; set; }

        [Display(Name = "Rooms")]
        public ICollection<RoomModel> Rooms { get; set; }

        [Display(Name = "Country")]
        public CountryModel Country { get; set; }
    }
}
