using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        public int PayType { get; set; }

        public int ReservationType { get; set; }

        public int ItemId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string TelNo1 { get; set; }

        [MaxLength(50)]
        public string TelNo2 { get; set; }

        [Required]
        [MaxLength(150)]
        public string Address1 { get; set; }

        [MaxLength(150)]
        public string Address2 { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(10)]
        public string PostCode { get; set; }

        public int? CountryId { get; set; }

        public bool Paied { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderId { get; set; }

        [Required]
        public float TotalAmount { get; set; }

        public string UserId { get; set; }

        public DateTime ReservationFrom { get; set; }

        public DateTime ReservationTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Guest> Guests { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public virtual Country Country { get; set; }
    }
}
