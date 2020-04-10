using AllWorldReservation.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class CountryModel
    {
        public int Id { get; set; }

        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code Required")]
        [MaxLength(2, ErrorMessage = "Max Lenght 2 Character")]
        public string Code { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Places Number")]
        public ICollection<PlaceModel> Places { get; set; }

        [Display(Name = "Reservations Number")]
        public ICollection<ReservationModel> Reservations { get; set; }

        [Display(Name = "Users Number")]
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
