﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.DAL.Entities
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public float PriceUSD { get; set; }

        public float PriceEGP { get; set; }

        public int Duration { get; set; }

        public DateTime? AvalibleFrom { get; set; }

        public DateTime? AvalibleTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? PhotoId { get; set; }

        public int? PlaceId { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual Place Place { get; set; }

    }
}
