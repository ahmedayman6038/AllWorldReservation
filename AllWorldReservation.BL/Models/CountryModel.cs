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

        [Required]
        [MaxLength(2)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Places Numbers")]
        public ICollection<PlaceModel> Places { get; set; }

        [Display(Name = "Reservations Numbers")]
        public ICollection<ReservationModel> Reservations { get; set; }

        [Display(Name = "Users Numbers")]
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
