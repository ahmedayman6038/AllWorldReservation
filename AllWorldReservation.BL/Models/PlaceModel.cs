using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class PlaceModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Country")]
        public CountryModel Country { get; set; }

        [Display(Name = "Hotels Numbers")]
        public ICollection<HotelModel> Hotels { get; set; }

        [Display(Name = "Tours Numbers")]
        public ICollection<TourModel> Tours { get; set; }

        [Display(Name = "Properties Numbers")]
        public ICollection<PropertyModel> properties { get; set; }
    }
}
