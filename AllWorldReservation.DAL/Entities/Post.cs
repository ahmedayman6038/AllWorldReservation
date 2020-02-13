using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public int? CategoryId { get; set; }

        public int? PhotoId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Category Category { get; set; }

        public virtual Photo Photo { get; set; }

    }
}
