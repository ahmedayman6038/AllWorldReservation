using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ReservationsController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;

        public ReservationsController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        // GET: Reservations
        public ActionResult Index()
        {
            var reservations = unitOfWork.ReservationRepository.Get().OrderByDescending(p => p.CreatedDate);
            return View(Mapper.Map<IEnumerable<ReservationModel>>(reservations));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reservation = unitOfWork.ReservationRepository.GetByID(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            var reservationModel = Mapper.Map<ReservationModel>(reservation);
            if(reservationModel.ReservationType == ReservationType.Hotel)
            {
                var hotel = unitOfWork.HotelRepository.GetByID(reservationModel.ItemId);
                reservationModel.ReservedItem = hotel.Name;
            }
            else if (reservationModel.ReservationType == ReservationType.Tour)
            {
                var tour = unitOfWork.TourRepository.GetByID(reservationModel.ItemId);
                reservationModel.ReservedItem = tour.Name;
            }
            else if (reservationModel.ReservationType == ReservationType.SunHotel)
            {
                reservationModel.ReservedItem = "Sun Hotel";
            }
            else
            {
                reservationModel.ReservedItem = "Unknown";
            }
            return View(reservationModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reservation = unitOfWork.ReservationRepository.GetByID(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            var guests = unitOfWork.GuestRepository.Get(g => g.ReservationId == id);
            foreach (var guest in guests)
            {
                unitOfWork.GuestRepository.Delete(guest);
            }
            unitOfWork.ReservationRepository.Delete(reservation);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}