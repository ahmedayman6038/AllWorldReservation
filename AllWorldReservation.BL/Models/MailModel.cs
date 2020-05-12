using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class MailModel
    {
        public MailModel()
        {
            this.Date = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Send By")]
        public string UserId { get; set; }

        [Display(Name = "Message Type")]
        public MailType MailType { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Send By")]
        public UserModel User { get; set; }
    }
}
