using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.BL.Models
{
    public class PropertyModel
    {
        public PropertyModel()
        {
            this.CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price In USD")]
        public float PriceUSD { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price In EGP")]
        public float PriceEGP { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Avalible To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AvalibleTo { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Property Type")]
        public PropertyType Type { get; set; }

        [Display(Name = "Approved")]
        public bool Approved { get; set; }

        [Display(Name = "Photo")]
        public int? PhotoId { get; set; }

        [Display(Name = "Place")]
        public int? PlaceId { get; set; }

        [Display(Name = "Posted By")]
        public string UserId { get; set; }

        [Display(Name = "Photo")]
        public virtual PhotoModel Photo { get; set; }

        [Display(Name = "Place")]
        public virtual PlaceModel Place { get; set; }

        [Display(Name = "Posted By")]
        public virtual UserModel User { get; set; }
    }
}
