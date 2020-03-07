using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Code { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        [Required]
        public int Duration { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? PhotoId { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual ICollection<Hotel> Hotels { get; set; }

    }
}
