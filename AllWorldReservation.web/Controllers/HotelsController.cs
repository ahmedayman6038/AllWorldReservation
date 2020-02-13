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

namespace AllWorldReservation.web.Controllers
{
    public class HotelsController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;
        private string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        public HotelsController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var hotels = unitOfWork.HotelRepository.Get().OrderByDescending(p => p.CreatedDate);
            return View(Mapper.Map<IEnumerable<HotelModel>>(hotels));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hotel = unitOfWork.HotelRepository.GetByID(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            var hotelModel = Mapper.Map<HotelModel>(hotel);
            return View(hotelModel);
        }

        public ActionResult GetPhotos(int? id, int? current)
        {
            var page = 1;
            if (id != null && id > 0)
            {
                page = (int)id;
            }
            var photos = unitOfWork.PhotoRepository.Get().OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.ImageList = photos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.currentPhoto = current ?? 0;
            return PartialView("_Photos");
        }

        public ActionResult Create()
        {
            var hotel = new HotelModel();
            return View(hotel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Rating,Location,PhotoId")] HotelModel hotelModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var hotel = Mapper.Map<Hotel>(hotelModel);
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
                        var photo = Mapper.Map<Photo>(photoModel);
                        unitOfWork.PhotoRepository.Insert(photo);
                        hotel.Photo = photo;
                    }
                    else
                    {
                        ModelState.AddModelError("Photo", "please select photo in these formats .jpg, .jpeg, .png");
                        return View(hotelModel);
                    }
                }
                unitOfWork.HotelRepository.Insert(hotel);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(hotelModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hotel = unitOfWork.HotelRepository.GetByID(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            var hotelModel = Mapper.Map<HotelModel>(hotel);
            return View(hotelModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Rating,Location,PhotoId")] HotelModel hotelModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var hotel = Mapper.Map<Hotel>(hotelModel);
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileExtension = extension.ToLower();
                    if (allowedExtensions.Contains(fileExtension))
                    {
                        var oldPhoto = unitOfWork.PhotoRepository.GetByID(hotelModel.PhotoId);
                        var uniqe = Guid.NewGuid();
                        string path = Path.Combine(Server.MapPath("~/Uploads"), uniqe + extension);
                        file.SaveAs(path);
                        var photoModel = new PhotoModel();
                        photoModel.Name = uniqe + extension;
                        var photo = Mapper.Map<Photo>(photoModel);
                        unitOfWork.PhotoRepository.Insert(photo);
                        hotel.Photo = photo;
                        if (oldPhoto != null)
                        {
                            string PhotoPath = Server.MapPath("~/Uploads/" + oldPhoto.Name);
                            if (System.IO.File.Exists(PhotoPath))
                            {
                                System.IO.File.Delete(PhotoPath);
                            }
                            unitOfWork.PhotoRepository.Delete(oldPhoto);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Photo", "please select photo in these formats .jpg, .jpeg, .png");
                        return View(hotelModel);
                    }
                }
                unitOfWork.HotelRepository.Update(hotel);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(hotelModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hotel = unitOfWork.HotelRepository.GetByID(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            var photo = unitOfWork.PhotoRepository.GetByID(hotel.PhotoId);
            if (photo != null)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            unitOfWork.HotelRepository.Delete(hotel);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}