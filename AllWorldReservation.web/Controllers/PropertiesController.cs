using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AutoMapper;
using Microsoft.AspNet.Identity;
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
    [Authorize(Roles = "Admin, Employee, Customer")]
    public class PropertiesController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        private string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        public PropertiesController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index(string type)
        {
            var user = context.Users.Find(User.Identity.GetUserId());
            var userRole = context.Roles.Find(user.Roles.SingleOrDefault().RoleId).Name;
            ViewBag.Role = userRole;
            var properties = new List<Property>();
            if(type != null && type.ToLower() == "share")
            {
                properties = unitOfWork.PropertyRepository.Get(p => p.UserId != null && p.Approved == false).OrderByDescending(p => p.CreatedDate).ToList();
            }
            else
            {
                if (userRole == "Customer")
                {
                    properties = unitOfWork.PropertyRepository.Get(p => p.UserId == user.Id).OrderByDescending(p => p.CreatedDate).ToList();
                }
                else
                {
                    properties = unitOfWork.PropertyRepository.Get(p => p.UserId == null || p.Approved == true).OrderByDescending(p => p.CreatedDate).ToList();
                }
            }
            return View(Mapper.Map<IEnumerable<PropertyModel>>(properties));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var property = unitOfWork.PropertyRepository.GetByID(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            var propertyModel = Mapper.Map<PropertyModel>(property);
            return View(propertyModel);
        }

        public ActionResult GetPhotos(int? id, int? page, int? current)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Property && p.ItemId == id).OrderByDescending(p => p.UploadDate);
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
            var property = unitOfWork.PropertyRepository.GetByID(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Property && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Property = property;
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
                        photoModel.Type = PhotoType.Property;
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
            return View(new PropertyModel() { Type = PropertyType.Sell });
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,PriceUSD,PriceEGP,Address,AvalibleFrom,AvalibleTo,Type,PhotoId,PlaceId")] PropertyModel propertyModel, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var user = context.Users.Find(User.Identity.GetUserId());
                var userRole = context.Roles.Find(user.Roles.SingleOrDefault().RoleId).Name;
                var property = Mapper.Map<Property>(propertyModel);
                if(userRole == "Customer")
                {
                    property.UserId = User.Identity.GetUserId();
                }
                unitOfWork.PropertyRepository.Insert(property);
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
                            photoModel.Type = PhotoType.Property;
                            photoModel.ItemId = property.Id;
                            var photo = Mapper.Map<Photo>(photoModel);
                            unitOfWork.PhotoRepository.Insert(photo);
                            if (first)
                            {
                                property.Photo = photo;
                                unitOfWork.PropertyRepository.Update(property);
                                first = false;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Photo", "please select photos in these formats .jpg, .jpeg, .png");
                            var placess = unitOfWork.PlaceRepository.Get();
                            ViewBag.PlaceId = new SelectList(placess, "Id", "Name");
                            return View(propertyModel);
                        }
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(propertyModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var property = unitOfWork.PropertyRepository.GetByID(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            var propertyModel = Mapper.Map<PropertyModel>(property);
            return View(propertyModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,PriceUSD,PriceEGP,Address,AvalibleFrom,AvalibleTo,Type,PhotoId,PlaceId,UserId,Approved")] PropertyModel propertyModel)
        {
            if (ModelState.IsValid)
            {
                var property = Mapper.Map<Property>(propertyModel);
                unitOfWork.PropertyRepository.Update(property);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(propertyModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Delete(int? id, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var property = unitOfWork.PropertyRepository.GetByID(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            // Delete Property Photos
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Property && p.ItemId == property.Id);
            foreach (var photo in photos)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            // Delete Property Reservations
            var reservations = unitOfWork.ReservationRepository.Get(r => r.ReservationType == (int)ReservationType.Property && r.ItemId == property.Id);
            foreach (var reservation in reservations)
            {
                unitOfWork.ReservationRepository.Delete(reservation);
            }
            unitOfWork.PropertyRepository.Delete(property);
            unitOfWork.Save();
            if (type != null && type.ToLower() == "share")
            {
                return RedirectToAction("Index", new { type = "share" });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var property = unitOfWork.PropertyRepository.GetByID(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            property.Approved = true;
            unitOfWork.PropertyRepository.Update(property);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}