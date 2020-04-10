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

        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code Required")]
        public string Code { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Country")]
        public CountryModel Country { get; set; }

        [Display(Name = "Hotels Numbers")]
        public ICollection<HotelModel> Hotels { get; set; }

        [Display(Name = "Tours Numbers")]
        public ICollection<TourModel> Tours { get; set; }
    }
}
