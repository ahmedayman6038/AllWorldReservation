using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class RoomModel
    {
        public int Id { get; set; }

        [Display(Name = "RoomRateID")]
        public int RateId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "BoardBasis")]
        public string BoardBasis { get; set; }

        [Display(Name = "TotalAmount")]
        public float TotalAmount { get; set; }
    }
}
