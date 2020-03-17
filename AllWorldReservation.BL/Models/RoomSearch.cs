﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Models
{
    public class RoomSearch
    {
        public int RoomNumber { get; set; }
        public int Adults { get; set; }
        public int Teenagers { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public List<int> ChildAges { get; set; }
    }
}
