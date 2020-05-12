using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class HotelModel
    {
        public HotelModel()
        {
            this.CreatedDate = DateTime.Now;
            this.Rooms = new List<RoomModel>();
        }

        public int Id { get; set; }

        public string ResultId { get; set; }

        public string GUID { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price From In USD")]
        public float PriceFromUSD { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price From In EGP")]
        public float PriceFromEGP { get; set; }

        [Display(Name = "Stars")]
        public int Stars { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleTo { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Place")]
        public int? PlaceId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Hotel Type")]
        public ReservationType Type { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }

        [Display(Name = "Place")]
        public PlaceModel Place { get; set; }

        [Display(Name = "Rooms Numbers")]
        public ICollection<RoomModel> Rooms { get; set; }
    }
}
