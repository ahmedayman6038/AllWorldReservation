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
    public class HotelsController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
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

        public ActionResult GetPhotos(int? id,int? page, int? current)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Hotel && p.ItemId == id).OrderByDescending(p => p.UploadDate);
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
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hotel = unitOfWork.HotelRepository.GetByID(id);
            if(hotel == null)
            {
                return HttpNotFound();
            }
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Hotel && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Hotel = hotel;
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
                        photoModel.Type = PhotoType.Hotel;
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
            var hotel = new HotelModel();
            hotel.Rooms = new List<RoomModel>();
            hotel.Rooms.Add(new RoomModel() { Guests = 1 });
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            return View(hotel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Stars,Address,AvalibleFrom,AvalibleTo,Rooms,PhotoId,PlaceId")] HotelModel hotelModel, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid && hotelModel.Rooms != null)
            {
                foreach (var item in hotelModel.Rooms.ToList())
                {
                    if (item.Deleted) hotelModel.Rooms.Remove(item);
                }
                hotelModel.PriceFromUSD = hotelModel.Rooms.Min(r => r.PriceUSD);
                hotelModel.PriceFromEGP = hotelModel.Rooms.Min(r => r.PriceEGP);
                var hotel = Mapper.Map<Hotel>(hotelModel);
                unitOfWork.HotelRepository.Insert(hotel);
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
                            photoModel.Type = PhotoType.Hotel;
                            photoModel.ItemId = hotel.Id;
                            var photo = Mapper.Map<Photo>(photoModel);
                            unitOfWork.PhotoRepository.Insert(photo);
                            if (first)
                            {
                                hotel.Photo = photo;
                                unitOfWork.HotelRepository.Update(hotel);
                                first = false;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Photo", "please select photos in these formats .jpg, .jpeg, .png");
                            var placess = unitOfWork.PlaceRepository.Get();
                            ViewBag.PlaceId = new SelectList(placess, "Id", "Name");
                            return View(hotelModel);
                        }
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
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
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
            var hotelModel = Mapper.Map<HotelModel>(hotel);
            return View(hotelModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Stars,Address,AvalibleFrom,AvalibleTo,Rooms,PhotoId,PlaceId")] HotelModel hotelModel)
        {
            if (ModelState.IsValid && hotelModel.Rooms != null)
            {
                var rooms = unitOfWork.RoomRepository.Get(r => r.HotelId == hotelModel.Id);
                foreach (var item in rooms)
                {
                    unitOfWork.RoomRepository.Delete(item);
                }
                var hotel = Mapper.Map<Hotel>(hotelModel);
                hotel.Rooms = null;
                hotel.PriceFromUSD = hotelModel.Rooms.Min(r => r.PriceUSD);
                hotel.PriceFromEGP = hotelModel.Rooms.Min(r => r.PriceEGP);
                unitOfWork.HotelRepository.Update(hotel);
                foreach (var item in hotelModel.Rooms.ToList())
                {
                    if (!item.Deleted)
                    {
                        var room = Mapper.Map<Room>(item);
                        room.HotelId = hotel.Id;
                        unitOfWork.RoomRepository.Insert(room);
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.PlaceId = new SelectList(places, "Id", "Name");
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
            // Delete Hotel Photos
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Hotel && p.ItemId == hotel.Id);
            foreach (var photo in photos)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            // Delete Hotel Rooms
            var rooms = unitOfWork.RoomRepository.Get(r => r.HotelId == hotel.Id);
            foreach (var room in rooms)
            {
                unitOfWork.RoomRepository.Delete(room);
            }
            // Delete Hotel Reservations
            var reservations = unitOfWork.ReservationRepository.Get(r => r.ReservationType == (int)ReservationType.Hotel && r.ItemId == hotel.Id);
            foreach (var reservation in reservations)
            {
                unitOfWork.ReservationRepository.Delete(reservation);
            }
            unitOfWork.HotelRepository.Delete(hotel);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}