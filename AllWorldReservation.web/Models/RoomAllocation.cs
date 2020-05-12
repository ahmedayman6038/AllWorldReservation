using AllWorldReservation.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllWorldReservation.web.Models
{
    public class RoomAllocation
    {
        public int Id { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public int Infants { get; set; }

        public List<int> ChildAges { get; set; }

        public List<RoomModel> Rooms { get; set; }
    }
}