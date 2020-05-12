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

        [Required]
        [MaxLength(50)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Posted By")]
        public string UserId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Category")]
        public CategoryModel Category { get; set; }

        [Display(Name = "Photo")]
        public PhotoModel Photo { get; set; }

        [Display(Name = "Posted By")]
        public UserModel User { get; set; }
    }
}
