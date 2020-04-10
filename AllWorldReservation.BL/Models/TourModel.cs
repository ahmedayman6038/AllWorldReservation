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

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price Required")]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        [Display(Name = "Days")]
        [Required(ErrorMessage = "Days Required")]
        [DataType(DataType.Duration)]
        public int Duration { get; set; }

        [Display(Name = "Avalible From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleFrom { get; set; }

        [Display(Name = "Avalible To")]
        [DataType(DataType.Date)]
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
