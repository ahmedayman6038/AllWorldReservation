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
    public class GalleriesController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;

        public GalleriesController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index(int? id)
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
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            var pagePhotos = photos.Skip((page - 1) * pageSize).Take(pageSize);
            return View(Mapper.Map<IEnumerable<PhotoModel>>(pagePhotos));
        }

        [HttpPost]
        public ActionResult Index(List<HttpPostedFileBase> files)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
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
                        var photo = Mapper.Map<Photo>(photoModel);
                        unitOfWork.PhotoRepository.Insert(photo);
                        unitOfWork.Save();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var photo = unitOfWork.PhotoRepository.GetByID(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
            if (System.IO.File.Exists(PhotoPath))
            {
                System.IO.File.Delete(PhotoPath);
            }
            unitOfWork.PostRepository.Get(x => x.PhotoId == photo.Id).ToList().ForEach(x => x.PhotoId = null);
            unitOfWork.HotelRepository.Get(x => x.PhotoId == photo.Id).ToList().ForEach(x => x.PhotoId = null);
            unitOfWork.PlaceRepository.Get(x => x.PhotoId == photo.Id).ToList().ForEach(x => x.PhotoId = null);
            unitOfWork.PhotoRepository.Delete(photo);
            unitOfWork.Save();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}