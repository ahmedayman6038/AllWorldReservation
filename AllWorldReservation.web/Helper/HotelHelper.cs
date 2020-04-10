using AllWorldReservation.BL.Models;
using AllWorldReservation.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.web.Helper
{
    public static class HotelHelper
    {
        public static bool CheckHotelSearchRoom(List<RoomModel> hotelRooms, List<SearchViewModel> rooms)
        {
            var check = true;
            foreach (var room in rooms)
            {
                var totalGuests = room.Adults + room.Teenagers + room.Infants + room.Children;
                if(!hotelRooms.Any(r => r.Guests == totalGuests))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }
    }
}
