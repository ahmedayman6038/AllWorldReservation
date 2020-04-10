using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        [MaxLength(256)]
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [MaxLength(150)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(50)]
        [Display(Name = "City")]
        public string City { get; set; }

        [MaxLength(10)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Country")]
        public CountryModel Country { get; set; }

        [Display(Name = "Reservation Numbers")]
        public ICollection<ReservationModel> Reservations { get; set; }

        [Display(Name = "Mail Numbers")]
        public ICollection<MailModel> Mails { get; set; }

        [Display(Name = "Post Numbers")]
        public ICollection<PostModel> Posts { get; set; }
    }
}
