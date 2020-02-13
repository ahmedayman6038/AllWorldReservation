using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AllWorldReservation.web.Controllers
{
    public class CategoriesController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;

        public CategoriesController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var categories = unitOfWork.CategoryRepository.Get();
            return View(Mapper.Map<IEnumerable<CategoryModel>>(categories));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var categoryModel = Mapper.Map<CategoryModel>(category);
            return View(categoryModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                var category = Mapper.Map<Category>(categoryModel);
                unitOfWork.CategoryRepository.Insert(category);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(categoryModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var categoryModel = Mapper.Map<CategoryModel>(category);
            return View(categoryModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                var category = Mapper.Map<Category>(categoryModel);
                unitOfWork.CategoryRepository.Update(category);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(categoryModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            unitOfWork.CategoryRepository.Delete(category);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}