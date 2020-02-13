using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class SettingModel
    {
        public int Id { get; set; }

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Email")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        [EmailAddress(ErrorMessage = "Company Email Required")]
        public string CompanyEmail { get; set; }

        [Display(Name = "Company Address")]
        [MaxLength(100, ErrorMessage = "Max Lenght 100 Character")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Company Phone")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string CompanyPhone { get; set; }

        [Display(Name = "Company Telephone")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string CompanyTelephone { get; set; }

        [Display(Name = "Company Fax")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string CompanyFax { get; set; }

        [Display(Name = "About Company")]
        [DataType(DataType.MultilineText)]
        [MaxLength(300, ErrorMessage = "Max Lenght 300 Character")]
        public string AboutCompany { get; set; }

        [Display(Name = "Facebook Url")]
        [MaxLength(200, ErrorMessage = "Max Lenght 200 Character")]
        public string FacebookUrl { get; set; }

        [Display(Name = "Twitter Url")]
        [MaxLength(200, ErrorMessage = "Max Lenght 200 Character")]
        public string TwitterUrl { get; set; }

        [Display(Name = "Instagram Url")]
        [MaxLength(200, ErrorMessage = "Max Lenght 200 Character")]
        public string InstagramUrl { get; set; }
    }
}
