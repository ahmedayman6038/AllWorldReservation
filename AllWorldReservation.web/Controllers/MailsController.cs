﻿using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    public class MailsController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;

        public MailsController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Sender()
        {
            var mails = unitOfWork.MailRepository.Get().OrderByDescending(m => m.Date);
            return View(Mapper.Map<IEnumerable<MailModel>>(mails).Where(m => m.MailType == MailType.Sender));
        }

        public ActionResult Receiver()
        {
            var mails = unitOfWork.MailRepository.Get().OrderByDescending(m => m.Date);
            return View(Mapper.Map<IEnumerable<MailModel>>(mails).Where(m => m.MailType == MailType.Receiver));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mail = unitOfWork.MailRepository.GetByID(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            var mailModel = Mapper.Map<MailModel>(mail);
            return View(mailModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Subject,Email,Message")] MailModel mailModel)
        {
            ModelState["Name"].Errors.Clear();
            if (ModelState.IsValid)
            {
                mailModel.Name = "Ahmed";
             //   mailModel.UserId = int.Parse(Session["userid"].ToString());
                mailModel.MailType = MailType.Sender;
                var mail = Mapper.Map<Mail>(mailModel);
                unitOfWork.MailRepository.Insert(mail);
                var message = new MailMessage();
                message.To.Add(new MailAddress(mail.Email));
                message.Subject = mail.Subject;
                message.Body = mail.Message;
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(message);
                    unitOfWork.Save();
                }
                return RedirectToAction("Sender");
            }
            return View(mailModel);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mail = unitOfWork.MailRepository.GetByID(id);
            var mailModel = Mapper.Map<MailModel>(mail);
            if (mail == null)
            {
                return HttpNotFound();
            }
            unitOfWork.MailRepository.Delete(mail);
            unitOfWork.Save();
            if (mailModel.MailType == MailType.Sender)
            {
                return RedirectToAction("Sender");
            }
            else
            {
                return RedirectToAction("Receiver");
            }
        }
    }
}