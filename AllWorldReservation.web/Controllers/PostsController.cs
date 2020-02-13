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
    public class PostsController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;
        private string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        public PostsController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var posts = unitOfWork.PostRepository.Get().OrderByDescending(p => p.CreatedDate);
            return View(Mapper.Map<IEnumerable<PostModel>>(posts));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = unitOfWork.PostRepository.GetByID(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            var postModel = Mapper.Map<PostModel>(post);
            return View(postModel);
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
            var post = new PostModel();
            var categories = unitOfWork.CategoryRepository.Get();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View(post);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Content,CategoryId,PhotoId")] PostModel postModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var post = Mapper.Map<Post>(postModel);
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
                        post.Photo = photo;
                    }
                    else
                    {
                        ModelState.AddModelError("Photo", "please select photo in these formats .jpg, .jpeg, .png");
                        return View(postModel);
                    }
                }
                unitOfWork.PostRepository.Insert(post);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var categories = unitOfWork.CategoryRepository.Get();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View(postModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = unitOfWork.PostRepository.GetByID(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            var categories = unitOfWork.CategoryRepository.Get();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            var postModel = Mapper.Map<PostModel>(post);
            return View(postModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,UserId,CategoryId,PhotoId")] PostModel postModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var post = Mapper.Map<Post>(postModel);
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileExtension = extension.ToLower();
                    if (allowedExtensions.Contains(fileExtension))
                    {
                        var oldPhoto = unitOfWork.PhotoRepository.GetByID(postModel.PhotoId);
                        var uniqe = Guid.NewGuid();
                        string path = Path.Combine(Server.MapPath("~/Uploads"), uniqe + extension);
                        file.SaveAs(path);
                        var photoModel = new PhotoModel();
                        photoModel.Name = uniqe + extension;
                        var photo = Mapper.Map<Photo>(photoModel);
                        unitOfWork.PhotoRepository.Insert(photo);
                        post.Photo = photo;
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
                        return View(postModel);
                    }
                }
                unitOfWork.PostRepository.Update(post);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            var categories = unitOfWork.CategoryRepository.Get();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View(postModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var post = unitOfWork.PostRepository.GetByID(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            var photo = unitOfWork.PhotoRepository.GetByID(post.PhotoId);
            if (photo != null)
            {
                string PhotoPath = Server.MapPath("~/Uploads/" + photo.Name);
                if (System.IO.File.Exists(PhotoPath))
                {
                    System.IO.File.Delete(PhotoPath);
                }
                unitOfWork.PhotoRepository.Delete(photo);
            }
            unitOfWork.PostRepository.Delete(post);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}