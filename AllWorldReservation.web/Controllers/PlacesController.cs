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
    public class PlacesController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;
        private string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        public PlacesController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var places = unitOfWork.PlaceRepository.Get().OrderByDescending(p => p.CreatedDate);
            return View(Mapper.Map<IEnumerable<PlaceModel>>(places));
        }

        public ActionResult Details(int? id)
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
            return View(placeModel);
        }

        public ActionResult GetPhotos(int? id,int? page,int? current)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Place && p.ItemId == id).OrderByDescending(p => p.UploadDate);
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
            var place = unitOfWork.PlaceRepository.GetByID(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Place && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Place = place;
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
                        photoModel.Type = PhotoType.Place;
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
            var place = new PlaceModel();
            return View(place);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Location,Duration,PhotoId")] PlaceModel placeModel, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var place = Mapper.Map<Place>(placeModel);
                unitOfWork.PlaceRepository.Insert(place);
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
                            photoModel.Type = PhotoType.Place;
                            photoModel.ItemId = place.Id;
                            var photo = Mapper.Map<Photo>(photoModel);
                            unitOfWork.PhotoRepository.Insert(photo);
                            if (first)
                            {
                                place.Photo = photo;
                                unitOfWork.PlaceRepository.Update(place);
                                first = false;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Photo", "please select photos in these formats .jpg, .jpeg, .png");
                            return View(placeModel);
                        }
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
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
            return View(placeModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Location,Duration,PhotoId")] PlaceModel placeModel)
        {
            if (ModelState.IsValid)
            {
                var place = Mapper.Map<Place>(placeModel);
                unitOfWork.PlaceRepository.Update(place);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
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
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Place && p.ItemId == place.Id);
            foreach (var photo in photos)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            unitOfWork.PlaceRepository.Delete(place);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}