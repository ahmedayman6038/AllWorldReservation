using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlacesController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;

        public PlacesController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var places = unitOfWork.PlaceRepository.Get().OrderByDescending(p => p.Id);
            return View(Mapper.Map<IEnumerable<PlaceModel>>(places));
        }

        public ActionResult Create()
        {
            var countries = unitOfWork.CountryRepository.Get();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Code,CountryId")] PlaceModel placeModel)
        {
            if (ModelState.IsValid)
            {
                var place = Mapper.Map<Place>(placeModel);
                unitOfWork.PlaceRepository.Insert(place);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var countries = unitOfWork.CountryRepository.Get();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name");
            return View(placeModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var place = unitOfWork.PlaceRepository.GetByID(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            var placeModel = Mapper.Map<PlaceModel>(place);
            var countries = unitOfWork.CountryRepository.Get();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name");
            return View(placeModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Code,CountryId")] PlaceModel placeModel)
        {
            if (ModelState.IsValid)
            {
                var place = Mapper.Map<Place>(placeModel);
                unitOfWork.PlaceRepository.Update(place);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var countries = unitOfWork.CountryRepository.Get();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name");
            return View(placeModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var place = unitOfWork.PlaceRepository.GetByID(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            var hotels = unitOfWork.HotelRepository.Get(h => h.PlaceId == place.Id);
            foreach (var hotel in hotels)
            {
                unitOfWork.HotelRepository.Delete(hotel);
            }
            var tours = unitOfWork.TourRepository.Get(h => h.PlaceId == place.Id);
            foreach (var tour in tours)
            {
                unitOfWork.TourRepository.Delete(tour);
            }
            unitOfWork.PlaceRepository.Delete(place);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}