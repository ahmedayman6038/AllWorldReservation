using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class HotelModel
    {
        public HotelModel()
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

        [Display(Name = "Stars")]
        public int Stars { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location Required")]
        public string Location { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }
    }
}
