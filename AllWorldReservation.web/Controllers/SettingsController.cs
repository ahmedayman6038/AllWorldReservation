using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;

        public SettingsController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        public ActionResult Index()
        {
            var setting = unitOfWork.SettingRepository.Get().SingleOrDefault();
            var settingModel = new SettingModel();
            if (setting != null)
            {
                settingModel = Mapper.Map<SettingModel>(setting);
            }
            var currency = ConfigurationManager.AppSettings["GATEWAY_CURRENCY"];
            if(currency == "EGP")
            {
                settingModel.Currency = Currency.EGP;
            }
            else
            {
                settingModel.Currency = Currency.USD;
            }
            return View(settingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,CompanyName,CompanyEmail,CompanyAddress,CompanyPhone,CompanyTelephone,CompanyFax,AboutCompany,FacebookUrl,TwitterUrl,InstagramUrl,Currency")] SettingModel settingModel)
        {
            if (ModelState.IsValid)
            {
                var oldSetting = unitOfWork.SettingRepository.Get(noTrack: true).SingleOrDefault();
                var setting = Mapper.Map<Setting>(settingModel);
                if (oldSetting != null)
                {
                    unitOfWork.SettingRepository.Update(setting);
                }
                else
                {
                    unitOfWork.SettingRepository.Insert(setting);
                }
                unitOfWork.Save();
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
                if (settingModel.Currency == Currency.EGP)
                {
                    webConfigApp.AppSettings.Settings["GATEWAY_CURRENCY"].Value = "EGP";
                }
                else
                {
                    webConfigApp.AppSettings.Settings["GATEWAY_CURRENCY"].Value = "USD";
                }
                webConfigApp.Save();
            }
            return View(settingModel);
        }
    }
}
