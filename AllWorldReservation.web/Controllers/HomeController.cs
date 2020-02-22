using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace AllWorldReservation.web.Controllers
{
    public class HomeController : Controller
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;
        string Baseurl = "http://sunxml.digital-trip.co.uk/accommodation.api";

        public HomeController()
        {
            unitOfWork = new UnitOfWork(context);
        }

        [Route("")]
        [Route("Home")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("About")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View();
        }

        [Route("Blog")]
        public ActionResult Blog(int? page)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var blogs = unitOfWork.PostRepository.Get(p => p.Category.Name.ToLower() == "blog").OrderByDescending(p => p.Id).ToList();
            var pageSize = 9;
            var totalRecord = blogs.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            var data = blogs.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(data);
        }

        [Route("Blog/{id}")]
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

        [Route("Hotel")]
        public ActionResult Hotels(int? page, string hotel,int? place, int? fromPrice, int? toPrice, int? star)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var hotels = unitOfWork.HotelRepository.Get(orderBy: e => e.OrderByDescending(z => z.Id));
            var pageSize = 9;
            if (hotel != null)
            {
                if (!string.IsNullOrEmpty(hotel))
                    hotels = hotels.Where(e => e.Name.ToLower().Contains(hotel.ToLower()));
                if (place != null)
                    hotels = hotels.Where(e => e.PlaceId == place);
                if (fromPrice != null && fromPrice != 0)
                    hotels = hotels.Where(e => e.Price >= fromPrice);
                if (toPrice != null && toPrice != 0)
                    hotels = hotels.Where(e => e.Price <= toPrice);
                if (star != null)
                    hotels = hotels.Where(e => e.Stars == star);
            }
            var totalRecord = hotels.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Hotel = hotel;
            ViewBag.SelectedPlace = place;
            ViewBag.FromPrice = fromPrice ?? 0;
            ViewBag.ToPrice = toPrice ?? 0;
            ViewBag.Stars = star;
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.Place = new SelectList(places, "Id", "Name", place);
            hotels = hotels.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(hotels);
        }

        [Route("Place")]
        public ActionResult Places(int? page, string place, int? fromPrice, int? toPrice)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var places = unitOfWork.PlaceRepository.Get(orderBy: e => e.OrderByDescending(z => z.Id));
            var pageSize = 9;
            if (place != null)
            {
                if (!string.IsNullOrEmpty(place))
                    places = places.Where(e => e.Name.ToLower().Contains(place.ToLower()));
                if (fromPrice != null && fromPrice != 0)
                    places = places.Where(e => e.Price >= fromPrice);
                if (toPrice != null && toPrice != 0)
                    places = places.Where(e => e.Price <= toPrice);
            }
            var totalRecord = places.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Place = place;
            ViewBag.FromPrice = fromPrice ?? 0;
            ViewBag.ToPrice = toPrice ?? 0;
            places = places.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(places);
        }

        [Route("Book")]
        public ActionResult Booking()
        {
            return View();
        }

        [Route("SearchPlace")]
        public ActionResult SearchPlace(int? page, string place, int? fromPrice, int? toPrice)
        {
            return View();
        }

        [Route("api")]
        public async Task<ActionResult> TestAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://sunxml.digital-trip.co.uk/accommodation.api");                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", "test|test12345");
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                DateTime checkIn = new DateTime(2020,3,9);
                DateTime checkOut = new DateTime(2020,3,15);
                string data = "<SearchRequest>" +
                                    "<LocationID>899</LocationID>" +
                                    "<CheckIn>"+checkIn.ToString("yyyy-MM-dd")+"</CheckIn>" +
                                    "<CheckOut>"+checkOut.ToString("yyyy-MM-dd")+"</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>2</Adults>" +
                                            "<Children>0</Children>" +
                                            "<Infants>0</Infants>" +
                                        "</RoomAllocation>" +
                                    "</RoomAllocations>" +
                                "</SearchRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                while (true)
                {
                    HttpResponseMessage Res = await client.PostAsync("", content);
                    if (Res.IsSuccessStatusCode)
                    {
                        var result = Res.Content.ReadAsStringAsync().Result;
                        var xmlResult = XDocument.Parse(result);
                        var isSuccess = xmlResult.Root.Element("IsSuccess");
                        if(isSuccess.Value == "false")
                        {
                            return Content(result, "application/xml");
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        return Content("Error");
                    }
                }
            }
        }
    }
}