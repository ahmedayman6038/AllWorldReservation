using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CompanyName { get; set; }

        [MaxLength(50)]
        public string CompanyEmail { get; set; }

        [MaxLength(100)]
        public string CompanyAddress { get; set; }

        [MaxLength(50)]
        public string CompanyPhone { get; set; }

        [MaxLength(50)]
        public string CompanyTelephone { get; set; }

        [MaxLength(50)]
        public string CompanyFax { get; set; }

        [MaxLength(300)]
        public string AboutCompany { get; set; }

        [MaxLength(200)]
        public string FacebookUrl { get; set; }

        [MaxLength(200)]
        public string TwitterUrl { get; set; }

        [MaxLength(200)]
        public string InstagramUrl { get; set; }

    }
}
