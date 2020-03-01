using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        [Required]
        public string ResultId { get; set; }
        [Required]
        public string GUID { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string TelNo1 { get; set; }
        public string TelNo2 { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        public string Country { get; set; }

        public IEnumerable<GuestModel> Guests { get; set; }

    }
}
