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
using AllWorldReservation.web.Helper;

namespace AllWorldReservation.web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private ApplicationDbContext context;
        private SystemHelper systemHelper;

        public DashboardController()
        {
            context = new ApplicationDbContext();
            systemHelper = new SystemHelper(context);
        }

        [Route("Dashboard")]
        public ActionResult Index()
        {
            var user = context.Users.Find(User.Identity.GetUserId());
            var userRole = context.Roles.Find(user.Roles.SingleOrDefault().RoleId).Name;
            ViewBag.Role = userRole;
            if (userRole == "Admin" || userRole == "Employee")
            {        
                ViewBag.Hotels = systemHelper.GetTotalHotels();
                ViewBag.Tours = systemHelper.GetTotalTours();
                ViewBag.Customers = systemHelper.GetTotalCustomers();
                ViewBag.Booking = systemHelper.GetTotalBooking();
            }
            else
            {
                ViewBag.UserBooking = systemHelper.GetUserTotalBooking(user.Id);
                ViewBag.BookingHotels = systemHelper.GetUserTotalHotelBooking(user.Id);
                ViewBag.BookingTours = systemHelper.GetUserTotalTourBooking(user.Id);
            }
            return View();
        }

    }
}