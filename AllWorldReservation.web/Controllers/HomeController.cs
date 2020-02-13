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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Hotels(int? page)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var hotels = unitOfWork.HotelRepository.Get().OrderByDescending(p => p.Id).ToList();
            var pageSize = 9;
            var totalRecord = hotels.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            var data = hotels.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(data);
        }

        [Route("Place")]
        public ActionResult Places(int? page)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var places = unitOfWork.PlaceRepository.Get().OrderByDescending(p => p.Id).ToList();
            var pageSize = 9;
            var totalRecord = places.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            var data = places.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(data);
        }

        [Route("Book")]
        public ActionResult Booking()
        {
            return View();
        }

        [Route("api")]
        public async Task<ActionResult> TestAsync()
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri("http://sunxml.digital-trip.co.uk/accommodation.api");                
                //Passing Header of the request
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", "test|test12345");
                client.DefaultRequestHeaders.Add("Action", "Search");
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                //The data that will be send with the request  
                var data = "<SearchRequest>" +
                                    "<LocationID>899</LocationID>" +
                                    "<CheckIn>2020-­03-­09</CheckIn>" +
                                    "<CheckOut>2020­-03-­15</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>2</Adults>" +
                                            "<Children>1</Children>" +
                                            "<Infants>0</Infants>" +
                                            "<ChildAges>" +
                                                "<int>5</int>" +
                                            "</ChildAges>" +
                                        "</RoomAllocation>" +
                                    "</RoomAllocations>" +
                                "</SearchRequest>";
                var content = new StringContent(data, Encoding.UTF8, "text/xml");
                //Send a post request with the content data to the service
                HttpResponseMessage Res = await client.PostAsync("", content);
                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var result = Res.Content.ReadAsStringAsync().Result;
                    //Return the result that's received from web api 
                    return Content(result, "application/xml");
                }
                //Return error message if the response not success
                return Content("error");
            }
        }
    }
}