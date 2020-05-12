using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class SettingModel
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [EmailAddress]
        [MaxLength(50)]
        [Display(Name = "Company Email")]
        public string CompanyEmail { get; set; }

        [MaxLength(100)]
        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }

        [MaxLength(50)]
        [Display(Name = "Company Phone")]
        public string CompanyPhone { get; set; }

        [MaxLength(50)]
        [Display(Name = "Company Telephone")]
        public string CompanyTelephone { get; set; }

        [MaxLength(50)]
        [Display(Name = "Company Fax")]
        public string CompanyFax { get; set; }

        [MaxLength(300)]
        [Display(Name = "About Company")]
        [DataType(DataType.MultilineText)]
        public string AboutCompany { get; set; }

        [MaxLength(200)]
        [Display(Name = "Facebook Url")]
        public string FacebookUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "Twitter Url")]
        public string TwitterUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "Instagram Url")]
        public string InstagramUrl { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public Currency Currency { get; set; }
    }
}
