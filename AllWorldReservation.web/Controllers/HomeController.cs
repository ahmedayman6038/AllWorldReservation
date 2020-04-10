using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.BL.Utils;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using AllWorldReservation.web.Gateway;
using AllWorldReservation.web.Helper;
using AllWorldReservation.web.Models;
using AutoMapper;
using System;
using System.Collections;
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
using Microsoft.AspNet.Identity;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        string Baseurl = "http://sunxml.digital-trip.co.uk/accommodation.api";
        string AuthCode = "test|test12345";
        private SunApiRequest sunApiRequest;

        public HomeController()
        {
            unitOfWork = new UnitOfWork(context);
            sunApiRequest = new SunApiRequest();
        }

        [Route("")]
        [Route("Home")]
        public ActionResult Index()
        {
            ViewBag.Destinations = new SelectList(unitOfWork.PlaceRepository.Get(), "Id", "Name");
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
            List<SearchViewModel> rooms = new List<SearchViewModel>();
            rooms.Add(new SearchViewModel() { Adults = 2 });
            Session["SRooms"] = rooms;
            ViewBag.CheckIn = DateTime.Now.AddDays(5);
            ViewBag.CheckOut = DateTime.Now.AddDays(6);
            var hotelsModel = Mapper.Map<List<HotelModel>>(hotels).ToList();
            foreach (var item in hotelsModel)
            {
                item.Price = item.Rooms.Count != 0 ? item.Rooms.ToList().Min(r => r.TotalAmount) : 0;
                item.ResultId = item.Id.ToString();
                item.Type = ReservationType.Hotel;
            }
            return View(hotelsModel);
        }

        [Route("Tour")]
        public ActionResult Tours(int? page, string tour, int? place, int? fromPrice, int? toPrice, int? days)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var tours = unitOfWork.TourRepository.Get(orderBy: e => e.OrderByDescending(z => z.Id));
            var pageSize = 9;
            if (tour != null)
            {
                if (!string.IsNullOrEmpty(tour))
                    tours = tours.Where(e => e.Name.ToLower().Contains(tour.ToLower()));
                if (place != null)
                    tours = tours.Where(e => e.PlaceId == place);
                if (fromPrice != null && fromPrice != 0)
                    tours = tours.Where(e => e.Price >= fromPrice);
                if (toPrice != null && toPrice != 0)
                    tours = tours.Where(e => e.Price <= toPrice);
                if (days != null)
                    tours = tours.Where(e => e.Duration == days);
            }
            var totalRecord = tours.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Tour = tour;
            ViewBag.SelectedPlace = place;
            ViewBag.FromPrice = fromPrice ?? 0;
            ViewBag.ToPrice = toPrice ?? 0;
            ViewBag.Days = days;
            SearchViewModel search = new SearchViewModel() { Adults = 2 };
            Session["STour"] = search;
            ViewBag.DateFrom = DateTime.Now.AddDays(1);
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.Place = new SelectList(places, "Id", "Name", place);
            tours = tours.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(Mapper.Map<List<TourModel>>(tours));
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
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                DateTime checkIn = new DateTime(2020, 3, 5);
                DateTime checkOut = new DateTime(2020, 3, 15);
                string data = "<SearchRequest>" +
                                    "<LocationID>17320</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>1</Adults>" +
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
                        var isComplete = xmlResult.Root.Element("IsComplete");
                        if (isSuccess.Value == "false" || (isComplete.Value == "true" && isSuccess.Value == "true"))
                        {
                            return Content(result, "application/xml");
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return Content("Error");
                    }
                }
            }
        }

        [Route("Search/Tour")]
        public ActionResult SearchTour(int? destination, DateTime dateFrom, DateTime dateTo, SearchViewModel traveller)
        {
            if (destination == null)
            {
                return RedirectToAction("Index");
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if (place == null)
            {
                return HttpNotFound();
            }
            var days = (dateTo - dateFrom).Days + 1;
            var allTours = unitOfWork.TourRepository.Get(h => h.PlaceId == place.Id && h.AvalibleFrom <= dateFrom && h.AvalibleTo >= dateTo && h.Duration == days);
            Session["STour"] = traveller;
            ViewBag.Location = place.Name;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.Destination = destination;
            return View(Mapper.Map<IEnumerable<TourModel>>(allTours));
        }

        [Route("Search/Hotel")]
        public async Task<ActionResult> SearchHotel(int? destination, DateTime checkIn, DateTime checkOut, List<SearchViewModel> rooms)
        {           
            if(destination == null)
            {
                return RedirectToAction("Index");
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if(place == null)
            {
                return HttpNotFound();
            }
            foreach (var room in rooms.ToList())
            {
                if (room.Deleted)
                {
                    rooms.Remove(room);
                }
            }
            var sunHotels = await sunApiRequest.SearchHotelAsync(place.Code, checkIn, checkOut, rooms);
            var allHotels = unitOfWork.HotelRepository.Get(h => h.PlaceId == place.Id && h.AvalibleFrom <= checkIn && h.AvalibleTo >= checkOut);
            var hotels = Mapper.Map<IEnumerable<HotelModel>>(allHotels).ToList();
            foreach (var hotel in hotels.ToList())
            {
                if(!HotelHelper.CheckHotelSearchRoom(hotel.Rooms.ToList(), rooms))
                {
                    hotels.Remove(hotel);
                    continue;
                }
                var prices = new List<float>();
                foreach (var room in hotel.Rooms.ToList())
                {
                    prices.Add(room.TotalAmount);
                }
                hotel.Price = prices.Min();
                hotel.ResultId = hotel.Id.ToString();
                hotel.Type = ReservationType.Hotel;
            }
            hotels.AddRange(sunHotels);
            Session["SRooms"] = rooms;
            ViewBag.Location = place.Name;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Destination = destination;
            return View(hotels);
        }

        [Route("Check/Tour")]
        public ActionResult CheckTour(int? destination, DateTime dateFrom, DateTime dateTo, int itemId)
        {
            if (destination == null)
            {
                return RedirectToAction("Index");
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if (place == null || Session["STour"] == null)
            {
                return HttpNotFound();
            }
            var search = (SearchViewModel)Session["STour"];
            var tour = Mapper.Map<TourModel>(unitOfWork.TourRepository.GetByID(itemId));
            int Adults = search.Adults, 
                Teenagers = search.Teenagers, 
                Children = search.Children, 
                Infants = search.Infants;
            IDictionary<string, int> traveller = new Dictionary<string, int>();
            traveller.Add("Adults", Adults);
            traveller.Add("Teenagers", Teenagers);
            traveller.Add("Children", Children);
            traveller.Add("Infants", Infants);
            ViewBag.Location = tour.Place.Name;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            Session["Traveller"] = traveller;
            Session["TotalCost"] = (Adults + Teenagers + Children) * tour.Price;
            return View(tour);
        }

        [Route("Check/Hotel")]
        public async Task<ActionResult> CheckHotel(int? destination, DateTime checkIn, DateTime checkOut, string itemId, ReservationType type)
        {           
            if (destination == null)
            {
                return RedirectToAction("Index");
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if (place == null || Session["SRooms"] ==null)
            {
                return HttpNotFound();
            }
            var rooms = (List<SearchViewModel>)Session["SRooms"];
            var hotel = new HotelModel();
            if(type == ReservationType.SunHotel)
            {
                hotel = await sunApiRequest.GetHotelAsync(place.Code, checkIn, checkOut, rooms, itemId);
                if(hotel == null) return HttpNotFound();
                hotel.Type = ReservationType.SunHotel;
            }
            else if(type == ReservationType.Hotel)
            {
                hotel = Mapper.Map<HotelModel>(unitOfWork.HotelRepository.GetByID(int.Parse(itemId)));
                if (hotel == null) return HttpNotFound();
                hotel.Type = ReservationType.Hotel;
                hotel.ResultId = hotel.Id.ToString();
                hotel.GUID = Guid.NewGuid().ToString();
            }
            int Adults=0 , Teenagers=0 ,Children=0 , Infants = 0;
            foreach (var room in rooms)
            {
                Adults += room.Adults;
                Teenagers += room.Teenagers;
                Children += room.Children;
                Infants += room.Infants;
            }
            IDictionary<string,int> guests = new Dictionary<string,int>();
            guests.Add("Adults", Adults);
            guests.Add("Teenagers", Teenagers);
            guests.Add("Children", Children);
            guests.Add("Infants", Infants);
            ViewBag.Rooms = rooms;
            ViewBag.Location = hotel.Place.Name;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            Session["Guests"] = guests;
            return View(hotel);
        }

        [Route("Book/Tour")]
        [HttpGet]
        public ActionResult BookTour(int ResultId, DateTime DateFrom, DateTime DateTo)
        {
            if (Session["Traveller"] == null || Session["TotalCost"] == null)
            {
                return HttpNotFound();
            }
            var totalCost = (float)Session["TotalCost"];
            if(totalCost == 0)
            {
                return HttpNotFound();
            }
            ViewBag.ResultId = ResultId;
            ViewBag.TotalCost = totalCost;
            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;
            ViewBag.Traveller = Session["Traveller"];
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            var reservation = new ReservationModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = context.Users.Find(User.Identity.GetUserId());
                reservation.Email = user.Email;
                reservation.TelNo1 = user.PhoneNumber;
                reservation.Address1 = user.Address;
                reservation.City = user.City;
                reservation.CountryId = user.CountryId;
                reservation.PostCode = user.PostCode;
                ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name", reservation.CountryId);
            }
            return View(reservation);
        }

        [Route("Book/Hotel")]
        [HttpGet]
        public ActionResult BookHotel(string Guid, string ResultId, DateTime CheckIn, DateTime CheckOut, ReservationType HotelType, List<int> rooms)
        {
            if(Session["Guests"] == null || rooms == null)
            {
                return HttpNotFound();
            }
            float totalCost = 0;
            foreach (var id in rooms)
            {
                var room = unitOfWork.RoomRepository.GetByID(id);
                if(room!= null)
                    totalCost += room.TotalAmount;
            }
            if(totalCost == 0)
            {
                return HttpNotFound();
            }
            ViewBag.GUID = Guid;
            ViewBag.ResultId = ResultId;
            ViewBag.TotalCost = totalCost;
            ViewBag.TotalRooms = rooms.Count;
            ViewBag.CheckIn = CheckIn;
            ViewBag.CheckOut = CheckOut;
            ViewBag.Guests = Session["Guests"];
            ViewBag.ResType = HotelType;
            Session["ResRooms"] = rooms;
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            var reservation = new ReservationModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = context.Users.Find(User.Identity.GetUserId());
                reservation.Email = user.Email;
                reservation.TelNo1 = user.PhoneNumber;
                reservation.Address1 = user.Address;
                reservation.City = user.City;
                reservation.CountryId = user.CountryId;
                reservation.PostCode = user.PostCode;
                ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name", reservation.CountryId);
            }
            return View(reservation);
        }

        [Route("Book/Tour")]
        [HttpPost]
        public ActionResult BookTour(int ResultId, ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var reservation = Mapper.Map<Reservation>(reservationModel);
                reservation.ItemId = ResultId;
                reservation.OrderId = IdUtils.generateSampleId();
                reservation.ReservationType = (int)ReservationType.Tour;
                if (User.Identity.IsAuthenticated)
                {
                    reservation.UserId = User.Identity.GetUserId();
                }
                unitOfWork.ReservationRepository.Insert(reservation);
                unitOfWork.Save();
                NotificationHelper.NotifySuccessBooking(reservation);
                if (reservationModel.PayType == PayType.Online)
                {
                    return Redirect("/Payment/" + reservation.Id);
                }
                else
                {
                    ViewBag.BookResult = "<h3>Success</h3><h5>Your booking Order : " + reservation.OrderId + " </h5>";
                    return View("BookResult");
                }
            }
            ViewBag.BookResult = "<h3>Faild</h3><h5>Please Enter Valid Data</h5>";
            return View("BookResult");
        }


        [Route("Book/Hotel")]
        [HttpPost]
        public async Task<ActionResult> BookHotelAsync(string GUID, string ResultId, ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var reservation = Mapper.Map<Reservation>(reservationModel);
                if(reservationModel.ReservationType == ReservationType.SunHotel)
                {
                    var country = unitOfWork.CountryRepository.GetByID(reservationModel.CountryId);
                    reservationModel.CountryCode = country.Code;
                    var bookRef = await sunApiRequest.BookHotelAsync(GUID, ResultId, reservationModel);
                    if (String.IsNullOrEmpty(bookRef))
                    {
                        return View("BookResult");
                    }
                    reservation.OrderId = bookRef;
                }
                else
                {
                    reservation.ItemId = int.Parse(ResultId);
                    reservation.OrderId = IdUtils.generateSampleId();
                    var roomsId = (List<int>)Session["ResRooms"];
                    foreach (var id in roomsId)
                    {
                        var room = unitOfWork.RoomRepository.GetByID(id);
                        if (room != null)
                            reservation.Rooms.Add(room);
                    }
                }
                if (User.Identity.IsAuthenticated)
                {
                    reservation.UserId = User.Identity.GetUserId();
                }
                unitOfWork.ReservationRepository.Insert(reservation);
                unitOfWork.Save();
                NotificationHelper.NotifySuccessBooking(reservation);
                if (reservationModel.PayType == PayType.Online)
                {
                    return Redirect("/Payment/" + reservation.Id);
                }
                else
                {
                    ViewBag.BookResult = "<h3>Success</h3><h5>Your booking Order : "+reservation.OrderId+" </h5>";
                    return View("BookResult");
                }
            }
            ViewBag.BookResult = "<h3>Faild</h3><h5>Please Enter Valid Data</h5>";
            return View("BookResult");
        }

        [Route("api2")]
        public async Task<ActionResult> TestAsync(string GUID, string ResultId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Book");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                DateTime dob = new DateTime(2020, 1, 1);
                string data = "<BookRequest>" +
                                    "<GUID>" + GUID + "</GUID>" +
                                    "<ResultID>" + ResultId + "</ResultID>" +
                                    "<Passengers>";
              
                    data += "<Passenger>" +
                                "<Title>" + "Mr" + "</Title>" +
                                "<Firstname>" + "Ahmed" + "</Firstname>" +
                                "<Surname>" + "Ayman" + "</Surname>" +
                                "<Type>Adult</Type>" +
                                "<DOB>" + dob.ToString("yyyy-MM-dd") + "</DOB>" +
                           "</Passenger>";
                
                data += "</Passengers>" +
                        "<Reference>MYREFERENCE</Reference>" +
                        "<CustomerTitle>" + "Mr" + "</CustomerTitle>" +
                        "<CustomerFName>" + "Ahmed" + "</CustomerFName>" +
                        "<CustomerSName>" + "Ayman" + "</CustomerSName>" +
                        "<CustomerAddress1>" + "Test Address 1" + "</CustomerAddress1>" +
                        "<CustomerAddress2>" + "Test Address 2" + "</CustomerAddress2>" +
                        "<CustomerCity>" + "Cairo" + "</CustomerCity>" +
                        "<CustomerPostCode>" + "55" + "</CustomerPostCode>" +
                        "<CustomerCountryCode>" + "EG" + "</CustomerCountryCode>" +
                        "<CustomerTelDay>" + "0122455" + "</CustomerTelDay>" +
                        "<CustomerTelEve>" + "1545454" + "</CustomerTelEve>" +
                        "<CustomerEmail>" + "ahmed@gmail.com" + "</CustomerEmail>" +
                        "</BookRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                HttpResponseMessage Res = await client.PostAsync("", content);
                if (Res.IsSuccessStatusCode)
                {
                    var result = Res.Content.ReadAsStringAsync().Result;
                    var xmlResult = XDocument.Parse(result);
                    var isSuccess = xmlResult.Root.Element("IsSuccess");
                    //var isComplete = xmlResult.Root.Element("IsComplete");
                    //if (isComplete.Value == "true" && isSuccess.Value == "true")
                    //{
                    //    var bookRef = xmlResult.Root.Element("BookingRef");
                    //    ViewBag.Result = "Success";
                    //    ViewBag.BookRef = bookRef.Value;
                    //    return View("BookResult");
                    //}
                    //else
                    //{
                    //    return Content("Booking Faild");
                    //}
                    return Content(result, "application/xml");
                }
                else
                {
                    return Content("Error");
                }
            }
        }
        //[Route("Check/Hotel")]
        //public async Task<ActionResult> CheckHotel(string Guid, string ResultId, int Adults, int RoomId, int RateId)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(Baseurl);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
        //        client.DefaultRequestHeaders.Add("Action", "Availability");
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        //        string data = "<AvailabilityRequest>" +
        //                            "<GUID>" + Guid + "</GUID>" +
        //                            "<Stage>Start</Stage>" +
        //                            "<ResultID>" + ResultId + "</ResultID>" +
        //                            "<RoomAllocations>" +
        //                                "<RoomAllocation>" +
        //                                    "<Adults>" + Adults + "</Adults>" +
        //                                    "<RoomID>" + RoomId + "</RoomID>" +
        //                                    "<RoomRateID>" + RateId + "</RoomRateID>" +
        //                                "</RoomAllocation>" +
        //                            "</RoomAllocations>" +
        //                        "</AvailabilityRequest>";
        //        var content = new StringContent(data, Encoding.UTF8, "application/xml");
        //        var hotel = new HotelModel();
        //        var location = "Hotel";
        //        while (true)
        //        {
        //            HttpResponseMessage Res = await client.PostAsync("", content);
        //            if (Res.IsSuccessStatusCode)
        //            {
        //                var result = Res.Content.ReadAsStringAsync().Result;
        //                var xmlResult = XDocument.Parse(result);
        //                var isSuccess = xmlResult.Root.Element("IsSuccess");
        //                var isComplete = xmlResult.Root.Element("IsComplete");
        //                if (isSuccess.Value == "false")
        //                {
        //                    ViewBag.Location = location;
        //                    return View(hotel);
        //                }
        //                else if (isComplete.Value == "true" && isSuccess.Value == "true")
        //                {
        //                    var results = xmlResult.Root.Element("Results");
        //                    var GUID = xmlResult.Root.Element("GUID").Value;
        //                    var accommodation = results.Element("AccommodationResult");
        //                    var resultId = accommodation.Element("ID").Value;
        //                    hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
        //                    hotel.ResultId = resultId;
        //                    hotel.GUID = GUID;
        //                    hotel.Location = accommodation.Element("Address").Value;
        //                    location = accommodation.Element("Location").Value;
        //                    hotel.Description = accommodation.Element("Description").Value;
        //                    foreach (var item in results.Elements())
        //                    {
        //                        var hotel = new HotelModel();
        //                        var resultId = item.Element("ID").Value;
        //                        hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
        //                        hotel.ResultId = resultId;
        //                        hotel.GUID = GUID;
        //                        hotel.Name = item.Element("Name").Value;
        //                        hotel.Location = item.Element("Address").Value;
        //                        location = item.Element("Location").Value;
        //                        hotel.Description = item.Element("Description").Value.Substring(0, 150) + "...";
        //                        var rooms = item.Element("RoomAllocations").Element("RoomAllocation").Element("Rooms").Elements();
        //                        var prices = new List<float>();
        //                        var first = true;
        //                        foreach (var room in rooms)
        //                        {
        //                            if (first)
        //                            {
        //                                roomId = int.Parse(room.Element("RoomID").Value);
        //                                rateId = int.Parse(room.Element("RoomRateID").Value);
        //                                first = false;
        //                            }
        //                            prices.Add(float.Parse(room.Element("TotalAmount").Value));
        //                        }
        //                        hotel.Price = prices.Min();
        //                        hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*", string.Empty));
        //                        hotels.Add(hotel);
        //                    }
        //                    ViewBag.Adults = guest;
        //                    ViewBag.RoomID = roomId;
        //                    ViewBag.RateID = rateId;
        //                    ViewBag.Location = location;
        //                    return View(hotels);
        //                }
        //                //var guid = xmlResult.Root.Element("GUID");
        //                var xmlRequest = XDocument.Parse(data);
        //                xmlRequest.Root.Element("Stage").Value = "Ping";
        //                content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
        //                Thread.Sleep(1000);
        //            }
        //            else
        //            {
        //                ViewBag.Location = location;
        //                return View(hotels);
        //            }
        //        }
        //    }
        //}
    }
}