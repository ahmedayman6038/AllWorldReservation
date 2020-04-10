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
    [Authorize(Roles = "Admin, Employee")]
    public class ToursController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        private string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        public ToursController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var tours = unitOfWork.TourRepository.Get().OrderByDescending(p => p.CreatedDate);
            return View(Mapper.Map<IEnumerable<TourModel>>(tours));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tour = unitOfWork.TourRepository.GetByID(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            var tourModel = Mapper.Map<TourModel>(tour);
            return View(tourModel);
        }

        public ActionResult GetPhotos(int? id, int? page, int? current)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Tour && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.ImageList = photos.Skip(((int)page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.currentPhoto = current ?? 0;
            return PartialView("_Photos");
        }

        public ActionResult Photos(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tour = unitOfWork.TourRepository.GetByID(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Tour && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Tour = tour;
            var pagePhotos = photos.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(Mapper.Map<IEnumerable<PhotoModel>>(pagePhotos));
        }

        [HttpPost]
        public ActionResult Photos(int? id, List<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileExtension = extension.ToLower();
                    if (allowedExtensions.Contains(fileExtension))
                    {
                        var uniqe = Guid.NewGuid();
                        string path = Path.Combine(Server.MapPath("~/Uploads"), uniqe + extension);
                        file.SaveAs(path);
                        var photoModel = new PhotoModel();
                        photoModel.Name = uniqe + extension;
                        photoModel.Type = PhotoType.Tour;
                        photoModel.ItemId = (int)id;
                        var photo = Mapper.Map<Photo>(photoModel);
                        unitOfWork.PhotoRepository.Insert(photo);
                        unitOfWork.Save();
                    }
                }
            }
            return RedirectToAction("Photos", new { id = id });
        }

        public ActionResult Create()
        {
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(new TourModel());
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Duration,AvalibleFrom,AvalibleTo,PlaceId,PhotoId")] TourModel tourModel, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var tour = Mapper.Map<Tour>(tourModel);
                unitOfWork.TourRepository.Insert(tour);
                unitOfWork.Save();
                var first = true;
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        var fileExtension = extension.ToLower();
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            var uniqe = Guid.NewGuid();
                            string path = Path.Combine(Server.MapPath("~/Uploads"), uniqe + extension);
                            file.SaveAs(path);
                            var photoModel = new PhotoModel();
                            photoModel.Name = uniqe + extension;
                            photoModel.Type = PhotoType.Tour;
                            photoModel.ItemId = tour.Id;
                            var photo = Mapper.Map<Photo>(photoModel);
                            unitOfWork.PhotoRepository.Insert(photo);
                            if (first)
                            {
                                tour.Photo = photo;
                                unitOfWork.TourRepository.Update(tour);
                                first = false;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Photo", "please select photos in these formats .jpg, .jpeg, .png");
                            var placess = unitOfWork.PlaceRepository.Get();
                            ViewBag.PlaceId = new SelectList(placess, "Id", "Name");
                            return View(tourModel);
                        }
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(tourModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tour = unitOfWork.TourRepository.GetByID(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            var tourModel = Mapper.Map<TourModel>(tour);
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(tourModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Duration,AvalibleFrom,AvalibleTo,PlaceId,PhotoId")] TourModel tourModel)
        {
            if (ModelState.IsValid)
            {
                var tour = Mapper.Map<Tour>(tourModel);
                unitOfWork.TourRepository.Update(tour);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(tourModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tour = unitOfWork.TourRepository.GetByID(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Tour && p.ItemId == tour.Id);
            foreach (var photo in photos)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            unitOfWork.TourRepository.Delete(tour);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}