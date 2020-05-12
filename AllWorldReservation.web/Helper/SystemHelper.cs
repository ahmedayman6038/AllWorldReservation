using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.web.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Helper
{
    public class SystemHelper
    {
        private ApplicationDbContext _context;
        private Currency _currency;

        public SystemHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public Currency Currency
        {
            get
            {

                var currency = ConfigurationManager.AppSettings["GATEWAY_CURRENCY"];
                if (currency == "EGP")
                {
                    return Currency.EGP;
                }
                else
                {
                    return Currency.USD;
                }
            }
            set
            {
                _currency = value;
            }
        }

        public bool CheckHotelSearchRoom(List<RoomModel> hotelRooms, List<GuestsViewModel> rooms)
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

        public List<HotelModel> GetTopHotels()
        {
            var result = new List<HotelModel>();
            var topReservation = _context.Reservations.Where(r => r.ReservationType == (int)ReservationType.Hotel)
                .GroupBy(g => g.ItemId).Select(group => new { ItemId = group.Key, Count = group.Count() })
                .OrderByDescending(r => r.Count).Take(4).ToList();
            foreach (var item in topReservation)
            {
                var hotel = Mapper.Map<HotelModel>(_context.Hotels.Find(item.ItemId));
                result.Add(hotel);
            }
            var remaining = 4 - topReservation.Count();
            if (remaining > 0)
            {
                var hotels = _context.Hotels.OrderByDescending(h => h.Stars).ToList();
                var top = hotels.Where(h => !result.Any(r => r.Id == h.Id)).Take(remaining);
                result.AddRange(Mapper.Map<List<HotelModel>>(top));
            }
            return result;
        }

        public List<TourModel> GetTopTours()
        {
            var result = new List<TourModel>();
            var topReservation = _context.Reservations.Where(r => r.ReservationType == (int)ReservationType.Tour)
                .GroupBy(g => g.ItemId).Select(group => new { ItemId = group.Key, Count = group.Count() })
                .OrderByDescending(r => r.Count).Take(4).ToList();
            foreach (var item in topReservation)
            {
                var tour = Mapper.Map<TourModel>(_context.Tours.Find(item.ItemId));
                result.Add(tour);
            }
            var remaining = 4 - topReservation.Count();
            if (remaining > 0)
            {
                var tours = _context.Tours.OrderByDescending(h => h.CreatedDate).ToList();
                var top = tours.Where(t => !result.Any(r => r.Id == t.Id)).Take(remaining);
                result.AddRange(Mapper.Map<List<TourModel>>(top));
            }
            return result;
        }

        public List<PostModel> GetTopArticles()
        {
            return Mapper.Map<List<PostModel>>(_context.Posts.Where(p => p.Category.Id == 3)
                .OrderByDescending(p => p.CreatedDate).Take(3).ToList());
        }

        public int GetTotalCustomers()
        {
            var customerRole = _context.Roles.Where(r => r.Name == "Customer").First().Id;
            return _context.Users.Where(u => u.Roles.FirstOrDefault().RoleId == customerRole).Count();
        }

        public int GetTotalBooking()
        {
            return _context.Reservations.Count();
        }

        public int GetTotalHotels()
        {
            return _context.Hotels.Count();
        }

        public int GetTotalTours()
        {
            return _context.Tours.Count();
        }

        public int GetUserTotalBooking(string userId)
        {
            return _context.Reservations.Where(r => r.UserId == userId).Count();
        }

        public int GetUserTotalHotelBooking(string userId)
        {
            return _context.Reservations.Where(r => r.UserId == userId && r.ReservationType == (int)ReservationType.Hotel).Count();
        }

        public int GetUserTotalTourBooking(string userId)
        {
            return _context.Reservations.Where(r => r.UserId == userId && r.ReservationType == (int)ReservationType.Tour).Count();
        }

       
    }
}
