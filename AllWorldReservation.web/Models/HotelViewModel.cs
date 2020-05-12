using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Models
{
    public class HotelViewModel
    {
        public int Id { get; set; }

        public string ResultId { get; set; }

        public string GUID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Place { get; set; }

        public string Photo { get; set; }

        public string Description { get; set; }

        public float PriceFromUSD { get; set; }

        public float PriceFromEGP { get; set; }

        public int Stars { get; set; }

        public ReservationType Type { get; set; }

        public List<RoomAllocation> RoomAllocations { get; set; }
    }
}