using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        [Route("Dashboard")]
        public ActionResult Index()
        {
            var user = context.Users.Find(User.Identity.GetUserId());
            var userRole = context.Roles.Find(user.Roles.SingleOrDefault().RoleId).Name;
            ViewBag.Role = userRole;
            if (userRole == "Admin" || userRole == "Employee")
            {
                var hotels = context.Hotels.Count();
                var tours = context.Tours.Count();
                var customerRole = context.Roles.Where(r => r.Name == "Customer").First().Id;
                var customers = context.Users.Where(u => u.Roles.FirstOrDefault().RoleId == customerRole).Count();
                var totalBooking = context.Reservations.Count();
                ViewBag.Hotels = hotels;
                ViewBag.Tours = tours;
                ViewBag.Customers = customers;
                ViewBag.Booking = totalBooking;
            }
            else
            {
                var userBooking = context.Reservations.Where(r => r.UserId == user.Id).Count();
                var userBookingHotels = context.Reservations.Where(r => r.UserId == user.Id && r.ReservationType == (int)ReservationType.Hotel).Count();
                var userBookingTours = context.Reservations.Where(r => r.UserId == user.Id && r.ReservationType == (int)ReservationType.Tour).Count();
                ViewBag.UserBooking = userBooking;
                ViewBag.BookingHotels = userBookingHotels;
                ViewBag.BookingTours = userBookingTours;
            }
            return View();
        }

    }
}