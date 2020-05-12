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
using Microsoft.AspNet.Identity;

namespace AllWorldReservation.web.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class PostsController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
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

        public ActionResult GetPhotos(int? id, int? page, int? current)
        {
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Post && p.ItemId == id).OrderByDescending(p => p.UploadDate);
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
            var post = unitOfWork.PostRepository.GetByID(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            if (page == null || page < 0)
            {
                page = 1;
            }
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Post && p.ItemId == id).OrderByDescending(p => p.UploadDate);
            var pageSize = 16;
            var totalRecord = photos.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Post = post;
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
                        photoModel.Type = PhotoType.Post;
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
            var categories = unitOfWork.CategoryRepository.Get();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Content,CategoryId,PhotoId")] PostModel postModel, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var post = Mapper.Map<Post>(postModel);
                post.UserId = User.Identity.GetUserId();
                unitOfWork.PostRepository.Insert(post);
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
                            photoModel.Type = PhotoType.Post;
                            photoModel.ItemId = post.Id;
                            var photo = Mapper.Map<Photo>(photoModel);
                            unitOfWork.PhotoRepository.Insert(photo);
                            if (first)
                            {
                                post.Photo = photo;
                                unitOfWork.PostRepository.Update(post);
                                first = false;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Photo", "please select photos in these formats .jpg, .jpeg, .png");
                            var categoriess = unitOfWork.CategoryRepository.Get();
                            ViewBag.CategoryId = new SelectList(categoriess, "Id", "Name");
                            return View(postModel);
                        }
                    }
                }
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
        public ActionResult Edit([Bind(Include = "Id,Title,Content,UserId,CategoryId,PhotoId")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                var post = Mapper.Map<Post>(postModel);
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
            // Delete Post Photos
            var photos = unitOfWork.PhotoRepository.Get(p => p.Type == (int)PhotoType.Post && p.ItemId == post.Id);
            foreach (var photo in photos)
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