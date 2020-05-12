using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AllWorldReservation.web.Models;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AllWorldReservation.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UsersController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var userModels = Mapper.Map<IEnumerable<UserModel>>(context.Users.ToList()).ToList();
            userModels.ForEach(u => u.Role = UserManager.GetRoles(u.Id).SingleOrDefault());
            return View(userModels);
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userModel = Mapper.Map<UserModel>(user);
            return View(userModel);
        }

        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(context.Roles.ToList(), "Name", "Name");
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Email,Password,Role,PhoneNumber,Address,CountryId,City,PostCode")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                //var user = Mapper.Map<ApplicationUser>(userModel);
                var user = new ApplicationUser {
                    UserName = userModel.UserName,
                    Email = userModel.Email,
                    PhoneNumber = userModel.PhoneNumber,
                    Address = userModel.Address,
                    CountryId = userModel.CountryId,
                    City = userModel.City,
                    PostCode = userModel.PostCode
                };
                var result = UserManager.Create(user, userModel.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, userModel.Role);
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(context.Roles.ToList(), "Name", "Name");
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            return View(userModel);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRole = UserManager.GetRoles(user.Id).SingleOrDefault();
            ViewBag.Role = new SelectList(context.Roles.ToList(), "Name", "Name", userRole);
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            var userModel = Mapper.Map<UserModel>(user);
            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Email,Password,Role,PhoneNumber,Address,CountryId,City,PostCode")] UserModel userModel)
        {
            ModelState["Password"].Errors.Clear();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(userModel.Id);
                var oldRole = UserManager.GetRoles(userModel.Id).SingleOrDefault();
                user.UserName = userModel.UserName;
                user.Email = userModel.Email;
                user.PhoneNumber = userModel.PhoneNumber;
                user.Address = userModel.Address;
                user.CountryId = userModel.CountryId;
                user.City = userModel.City;
                user.PostCode = userModel.PostCode;
                var result = UserManager.Update(user);
                if (result.Succeeded)
                {
                    if (oldRole != userModel.Role)
                    {
                        UserManager.RemoveFromRole(user.Id, oldRole);
                        UserManager.AddToRole(user.Id, userModel.Role);
                    }
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(context.Roles.ToList(), "Name", "Name");
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var role = UserManager.GetRoles(user.Id).First();
            // Delete Admin & Employees Posts & Mails
            if(role != "Customer")
            {
                var posts = context.Posts.Where(p => p.UserId == user.Id);
                context.Posts.RemoveRange(posts);
                var mails = context.Mails.Where(m => m.UserId == user.Id);
                context.Mails.RemoveRange(mails);
            }
            // Delete User Reservations
            var reservations = context.Reservations.Where(r => r.UserId == user.Id).ToList();
            foreach (var reservation in reservations)
            {
                var guests = context.Guests.Where(g => g.ReservationId == reservation.Id);
                context.Guests.RemoveRange(guests);
                context.Reservations.Remove(reservation);
            }
            context.Users.Remove(user);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindById(id);
            if(user == null)
            {
                return HttpNotFound();
            }
            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var reset = new ResetPasswordViewModel();
            reset.Code = code;
            reset.Email = user.Email;
            return View(reset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword([Bind(Include = "Email,Password,ConfirmPassword,Code")]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = UserManager.ResetPassword(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
