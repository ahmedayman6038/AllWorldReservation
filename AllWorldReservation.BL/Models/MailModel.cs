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

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Not Valid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Subject Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message Required")]
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
