using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class PhotoModel
    {
        public PhotoModel()
        {
            this.UploadDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(100, ErrorMessage = "Max Lenght 100 Character")]
        public string Name { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }

        [Display(Name = "Item Id")]
        public int ItemId { get; set; }

        [Display(Name = "Type")]
        public PhotoType Type { get; set; }
    }
}
