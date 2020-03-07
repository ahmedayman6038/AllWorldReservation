using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class PostModel
    {
        public PostModel()
        {
            this.CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title Required")]
        [MaxLength(50, ErrorMessage = "Max Lenght 50 Character")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category Required")]
        public int? CategoryId { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        //[Display(Name = "Post By")]
        //public int? UserId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Category")]
        public CategoryModel Category { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }

        //[Display(Name = "Post By")]
        //public UserModel User { get; set; }
    }
}
