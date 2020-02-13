using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AllWorldReservation.web.Controllers
{
    public class DashboardController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;

        public DashboardController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        [Route("Dashboard")]
        public ActionResult Index()
        {
            return View();
        }

    }
}