using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class TourModel
    {
        public TourModel()
        {
            this.CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

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

        [Required]
        [Display(Name = "Days")]
        [DataType(DataType.Duration)]
        [Range(minimum: 1, maximum: 30)]
        public int Duration { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleTo { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Place")]
        public int? PlaceId { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }

        [Display(Name = "Place")]
        public PlaceModel Place { get; set; }
    }
}
