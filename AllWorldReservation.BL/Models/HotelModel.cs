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

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        [Display(Name = "Stars")]
        public int Stars { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address Required")]
        [MaxLength(150, ErrorMessage = "Max Lenght 150 Character")]
        public string Address { get; set; }

        [Display(Name = "Avalible From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleFrom { get; set; }

        [Display(Name = "Avalible To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleTo { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Place")]
        [Required(ErrorMessage = "Place Required")]
        public int? PlaceId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Hotel Type")]
        public ReservationType Type { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }

        [Display(Name = "Place")]
        public PlaceModel Place { get; set; }

        public ICollection<RoomModel> Rooms { get; set; }
    }
}
