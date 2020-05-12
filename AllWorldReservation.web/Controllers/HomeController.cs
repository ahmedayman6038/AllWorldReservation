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
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace AllWorldReservation.web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        string Baseurl = "http://sunxml.digital-trip.co.uk/accommodation.api";
        string AuthCode = "test|test12345";
        private SunApiRequest sunApiRequest;
        private SystemHelper systemHelper;

        public HomeController()
        {
            unitOfWork = new UnitOfWork(context);
            sunApiRequest = new SunApiRequest();
            systemHelper = new SystemHelper(context);
        }

        [Route("")]
        [Route("Home")]
        public ActionResult Index()
        {
            ViewBag.Countries = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            ViewBag.TopHotels = systemHelper.GetTopHotels();
            ViewBag.TopTours = systemHelper.GetTopTours();
            ViewBag.TopArticles = systemHelper.GetTopArticles();
            ViewBag.Customers = systemHelper.GetTotalCustomers();
            ViewBag.TotalBooking = systemHelper.GetTotalBooking();
            ViewBag.TotalHotels = systemHelper.GetTotalHotels();
            ViewBag.TotalTours = systemHelper.GetTotalTours();
            ViewBag.CheckIn = DateTime.Now.AddDays(1);
            ViewBag.CheckOut = DateTime.Now.AddDays(2);
            ViewBag.Currency = systemHelper.Currency;
            List<GuestsViewModel> rooms = new List<GuestsViewModel>();
            GuestsViewModel guest = new GuestsViewModel() { Adults = 2 };
            rooms.Add(guest);
            Session["RoomsAllocation"] = rooms;
            Session["Traveller"] = guest;
            return View(Mapper.Map<List<PostModel>>(unitOfWork.PostRepository.Get(p => p.Category.Id == 4)));
        }

        [Route("GetPlaces/{id}")]
        public ActionResult GetPlaces(int id)
        {
            var places = unitOfWork.PlaceRepository.Get(p => p.CountryId == id)
                .Select(p => new { Id = p.Id, Name = p.Name }).ToList();
            return Json(places, JsonRequestBehavior.AllowGet);
        }

        [Route("About")]
        public ActionResult About()
        {
            ViewBag.Customers = systemHelper.GetTotalCustomers();
            ViewBag.TotalBooking = systemHelper.GetTotalBooking();
            ViewBag.TotalHotels = systemHelper.GetTotalHotels();
            ViewBag.TotalTours = systemHelper.GetTotalTours();
            return View(Mapper.Map<List<PostModel>>(unitOfWork.PostRepository.Get(p => p.Category.Id == 4)));
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View(Mapper.Map<SettingModel>(unitOfWork.SettingRepository.Get().First()));
        }

        [HttpPost]
        [Route("Home/SendMail")]
        public ActionResult SnedMail([Bind(Include = "Name,Subject,Email,Message")] MailModel mailModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    mailModel.MailType = MailType.Receiver;
                    var mail = Mapper.Map<Mail>(mailModel);
                    unitOfWork.MailRepository.Insert(mail);
                    var roles = context.Roles.Where(r => r.Name != "Customer").Select(r => r.Id);
                    var emails = context.Users.Where(u => roles.Any(r => r == u.Roles.FirstOrDefault().RoleId)).Select(u => u.Email).ToList();
                    NotificationHelper.NotifyUsersByMail(emails, mail.Subject, mail.Message);
                    unitOfWork.Save();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Route("Blog")]
        public ActionResult Blog(int? page)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var blogs = unitOfWork.PostRepository.Get(p => p.Category.Id == 3).OrderByDescending(p => p.Id).ToList();
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
                {
                    hotels = hotels.Where(e => e.Name.ToLower().Contains(hotel.ToLower()));
                }
                if (place != null)
                {
                    hotels = hotels.Where(e => e.PlaceId == place);
                }
                if (fromPrice != null && fromPrice != 0)
                {
                    if(systemHelper.Currency == Currency.EGP)
                    {
                        hotels = hotels.Where(e => e.PriceFromEGP >= fromPrice);
                    }
                    else
                    {
                        hotels = hotels.Where(e => e.PriceFromUSD >= fromPrice);
                    }
                }
                if (toPrice != null && toPrice != 0)
                {
                    if (systemHelper.Currency == Currency.EGP)
                    {
                        hotels = hotels.Where(e => e.PriceFromEGP <= toPrice);
                    }
                    else
                    {
                        hotels = hotels.Where(e => e.PriceFromUSD <= toPrice);
                    }
                }
                if (star != null)
                {
                    hotels = hotels.Where(e => e.Stars == star);
                }
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
            ViewBag.FromPrice = fromPrice;
            ViewBag.ToPrice = toPrice;
            ViewBag.Stars = star;
            ViewBag.Currency = systemHelper.Currency;
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.Place = new SelectList(places, "Id", "Name", place);
            hotels = hotels.Skip(((int)page - 1) * pageSize).Take(pageSize);
            List<GuestsViewModel> rooms = new List<GuestsViewModel>();
            rooms.Add(new GuestsViewModel() { Adults = 2 });
            Session["RoomsAllocation"] = rooms;
            ViewBag.CheckIn = DateTime.Now.AddDays(1);
            ViewBag.CheckOut = DateTime.Now.AddDays(2);
            var hotelsModel = Mapper.Map<List<HotelModel>>(hotels);
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
                {
                    tours = tours.Where(e => e.Name.ToLower().Contains(tour.ToLower()));
                }
                if (place != null)
                {
                    tours = tours.Where(e => e.PlaceId == place);
                }
                if (fromPrice != null && fromPrice != 0)
                {
                    if(systemHelper.Currency == Currency.EGP)
                    {
                        tours = tours.Where(e => e.PriceEGP >= fromPrice);
                    }
                    else
                    {
                        tours = tours.Where(e => e.PriceUSD >= fromPrice);
                    }
                }
                if (toPrice != null && toPrice != 0)
                {
                    if (systemHelper.Currency == Currency.EGP)
                    {
                        tours = tours.Where(e => e.PriceEGP <= toPrice);
                    }
                    else
                    {
                        tours = tours.Where(e => e.PriceUSD <= toPrice);
                    }
                }
                if (days != null)
                {
                    tours = tours.Where(e => e.Duration == days);
                }
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
            ViewBag.FromPrice = fromPrice;
            ViewBag.ToPrice = toPrice;
            ViewBag.Days = days;
            ViewBag.Currency = systemHelper.Currency;
            GuestsViewModel search = new GuestsViewModel() { Adults = 2 };
            Session["Traveller"] = search;
            ViewBag.DateFrom = DateTime.Now.AddDays(1);
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.Place = new SelectList(places, "Id", "Name", place);
            tours = tours.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(Mapper.Map<List<TourModel>>(tours));
        }

        [Route("Property")]
        public ActionResult Properties(int? page, string property, int? place, int? fromPrice, int? toPrice, int? type)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            var properties = unitOfWork.PropertyRepository.Get(filter: p => p.UserId == null || p.Approved == true, orderBy: e => e.OrderByDescending(z => z.Id));
            var pageSize = 9;
            if (property != null)
            {
                if (!string.IsNullOrEmpty(property))
                {
                    properties = properties.Where(e => e.Name.ToLower().Contains(property.ToLower()));
                }
                if (place != null)
                {
                    properties = properties.Where(e => e.PlaceId == place);
                }
                if (fromPrice != null && fromPrice != 0)
                {
                    if (systemHelper.Currency == Currency.EGP)
                    {
                        properties = properties.Where(e => e.PriceEGP >= fromPrice);
                    }
                    else
                    {
                        properties = properties.Where(e => e.PriceUSD >= fromPrice);
                    }
                }
                if (toPrice != null && toPrice != 0)
                {
                    if (systemHelper.Currency == Currency.EGP)
                    {
                        properties = properties.Where(e => e.PriceEGP <= toPrice);
                    }
                    else
                    {
                        properties = properties.Where(e => e.PriceUSD <= toPrice);
                    }
                }
                if (type != null)
                {
                    properties = properties.Where(e => e.Type == type);
                }
            }
            var totalRecord = properties.Count();
            var totalPages = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (page > totalPages)
            {
                page = totalPages;
            }
            ViewBag.totalPage = totalPages;
            ViewBag.currentPage = page;
            ViewBag.Property = property;
            ViewBag.SelectedPlace = place;
            ViewBag.FromPrice = fromPrice;
            ViewBag.ToPrice = toPrice;
            ViewBag.Type = type;
            ViewBag.Currency = systemHelper.Currency;
            var places = unitOfWork.PlaceRepository.Get();
            ViewBag.Place = new SelectList(places, "Id", "Name", place);
            properties = properties.Skip(((int)page - 1) * pageSize).Take(pageSize);
            return View(Mapper.Map<List<PropertyModel>>(properties));
        }

        [Route("Search/Tour")]
        public ActionResult SearchTour(int? destination, DateTime dateFrom, DateTime dateTo, GuestsViewModel traveller)
        {
            if (destination == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if (place == null)
            {
                return HttpNotFound();
            }
            var days = (dateTo - dateFrom).Days + 1;
            var allTours = unitOfWork.TourRepository.Get(h => h.PlaceId == place.Id && h.AvalibleFrom <= dateFrom && h.AvalibleTo >= dateTo && h.Duration == days);
            Session["Traveller"] = traveller;
            ViewBag.Location = place.Name;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.Destination = destination;
            ViewBag.Currency = systemHelper.Currency;
            return View(Mapper.Map<IEnumerable<TourModel>>(allTours));
        }

        [Route("Search/Hotel")]
        public async Task<ActionResult> SearchHotel(int? destination, DateTime checkIn, DateTime checkOut, List<GuestsViewModel> rooms)
        {           
            if(destination == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                if(!systemHelper.CheckHotelSearchRoom(hotel.Rooms.ToList(), rooms))
                {
                    hotels.Remove(hotel);
                    continue;
                }
                hotel.ResultId = hotel.Id.ToString();
                hotel.Type = ReservationType.Hotel;
            }
            hotels.AddRange(sunHotels);
            Session["RoomsAllocation"] = rooms;
            ViewBag.Location = place.Name;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Destination = destination;
            ViewBag.Currency = systemHelper.Currency;
            return View(hotels);
        }

        [Route("Check/Tour")]
        public ActionResult CheckTour(DateTime dateFrom, DateTime dateTo, int itemId)
        {
            var tour = unitOfWork.TourRepository.GetByID(itemId);
            if (tour == null)
            {
                return HttpNotFound();
            }
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            return View(Mapper.Map<TourModel>(tour));
        }

        [Route("Check/Property")]
        public ActionResult CheckProperty(int itemId)
        {
            var property = unitOfWork.PropertyRepository.GetByID(itemId);
            if (property == null)
            {
                return HttpNotFound();
            }
            return View(Mapper.Map<PropertyModel>(property));
        }

        [Route("Check/Hotel")]
        public async Task<ActionResult> CheckHotel(int? destination, DateTime checkIn, DateTime checkOut, string itemId, ReservationType type)
        {           
            if (destination == null || Session["RoomsAllocation"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var place = unitOfWork.PlaceRepository.GetByID(destination);
            if (place == null)
            {
                return HttpNotFound();
            }
            var rooms = (List<GuestsViewModel>)Session["RoomsAllocation"];
            var hotelModel = new HotelViewModel();
            if(type == ReservationType.SunHotel)
            {
                hotelModel = await sunApiRequest.GetHotelAsync(place.Code, checkIn, checkOut, rooms, itemId);
                if(hotelModel == null) return HttpNotFound();
            }
            else if(type == ReservationType.Hotel)
            {
                var hotel = unitOfWork.HotelRepository.GetByID(int.Parse(itemId));
                if (hotel == null) return HttpNotFound();
                hotelModel.Id = hotel.Id;
                hotelModel.GUID = Guid.NewGuid().ToString();
                hotelModel.ResultId = hotel.Id.ToString();
                hotelModel.Name = hotel.Name;
                hotelModel.Place = hotel.Place.Name;
                hotelModel.PriceFromUSD = hotel.PriceFromUSD;
                hotelModel.PriceFromEGP = hotel.PriceFromEGP;
                hotelModel.Address = hotel.Address;
                hotelModel.Stars = hotel.Stars;
                hotelModel.Description = hotel.Description;
                hotelModel.Photo = hotel.Photo != null ? hotel.Photo.Name : null;
                hotelModel.Type = ReservationType.Hotel;
                hotelModel.RoomAllocations = new List<RoomAllocation>();
                foreach (var room in rooms)
                {
                    var roomAllocation = new RoomAllocation();
                    roomAllocation.Adults = room.Adults + room.Teenagers;
                    roomAllocation.Children = room.Children;
                    roomAllocation.Infants = room.Infants;
                    var totalGuests = room.Adults + room.Teenagers + room.Children + room.Infants;
                    roomAllocation.Rooms = Mapper.Map<List<RoomModel>>(hotel.Rooms.Where(r => r.Guests == totalGuests));
                    hotelModel.RoomAllocations.Add(roomAllocation);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.Currency = systemHelper.Currency;
            return View(hotelModel);
        }

        [Route("Book/Tour")]
        [HttpGet]
        public ActionResult BookTour(int ResultId, DateTime DateFrom, DateTime DateTo)
        {
            if (Session["Traveller"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tour = unitOfWork.TourRepository.GetByID(ResultId);
            if(tour == null)
            {
                return HttpNotFound();
            }
            var available = false;
            if (tour.AvalibleFrom <= DateFrom && tour.AvalibleTo >= DateTo)
            {
                available = true;
            }
            if (!available)
            {
                ViewBag.BookResult = "<h3>Info</h3><h5>This item not available now</h5>";
                return View("BookResult");
            }
            var traveller = (GuestsViewModel)Session["Traveller"];
            float totalCost = 0;
            if (systemHelper.Currency == Currency.EGP)
            {
                totalCost = (traveller.Adults + traveller.Teenagers + traveller.Children) * tour.PriceEGP;
            }
            else
            {
                totalCost = (traveller.Adults + traveller.Teenagers + traveller.Children) * tour.PriceUSD;
            }          
            if (totalCost == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Adults = traveller.Adults;
            ViewBag.Teenagers = traveller.Teenagers;
            ViewBag.Children = traveller.Children;
            ViewBag.Infants = traveller.Infants;
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            var reservation = new ReservationModel();
            reservation.ItemId = ResultId;
            reservation.ReservationFrom = DateFrom;
            reservation.ReservationTo = DateTo;
            reservation.TotalAmount = totalCost;
            reservation.Currency = systemHelper.Currency;
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

        [Route("Book/Property")]
        [HttpGet]
        public ActionResult BookProperty(int ResultId)
        {
            var property = unitOfWork.PropertyRepository.GetByID(ResultId);
            if (property == null)
            {
                return HttpNotFound();
            }
            var available = false;
            if (property.AvalibleFrom <= DateTime.Now && property.AvalibleTo >= DateTime.Now)
            {
                available = true;
            }
            if (!available)
            {
                ViewBag.BookResult = "<h3>Info</h3><h5>This item not available now</h5>";
                return View("BookResult");
            }
            float totalCost = 0;
            if (systemHelper.Currency == Currency.EGP)
            {
                totalCost = property.PriceEGP;
            }
            else
            {
                totalCost = property.PriceUSD;
            }
            if (totalCost == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            ViewBag.PropertyType = property.Type;
            var reservation = new ReservationModel();
            reservation.ItemId = ResultId;
            reservation.TotalAmount = totalCost;
            reservation.Currency = systemHelper.Currency;
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
        public async Task<ActionResult> BookHotel(string Guid, string ResultId, DateTime CheckIn, DateTime CheckOut, ReservationType HotelType, List<string> rooms)
        {
            if(Session["RoomsAllocation"] == null || rooms == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var isUSD = false;
            if (systemHelper.Currency == Currency.USD || HotelType == ReservationType.SunHotel)
            {
                isUSD = true;
            }
            var reservedRooms = new List<RoomModel>();
            foreach (var item in rooms)
            {
                var roomModel = new RoomModel();
                var obj = JObject.Parse(item);
                roomModel.Id = int.Parse(obj["Id"].ToString());
                roomModel.RateId = int.Parse(obj["RateId"].ToString());
                if(isUSD)
                {
                    roomModel.PriceUSD = float.Parse(obj["TotalAmount"].ToString());
                }
                else
                {
                    roomModel.PriceEGP = float.Parse(obj["TotalAmount"].ToString());
                }
                reservedRooms.Add(roomModel);
            }
            Session["ReservedRooms"] = reservedRooms;
            var roomsAllocation = (List<GuestsViewModel>)Session["RoomsAllocation"];
            float totalCost = 0;
            if(isUSD)
            {
                totalCost = reservedRooms.Sum(r => r.PriceUSD);
            }
            else
            {
                totalCost = reservedRooms.Sum(r => r.PriceEGP);
            }
            if (totalCost == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var available = false;
            if (HotelType == ReservationType.Hotel)
            {
                var hotel = unitOfWork.HotelRepository.GetByID(int.Parse(ResultId));
                if (hotel == null)
                    return HttpNotFound();
                if(hotel.AvalibleFrom <= CheckIn && hotel.AvalibleTo >= CheckOut)
                {
                    available = true;
                }
            }
            else if(HotelType == ReservationType.SunHotel)
            {
                var allocations = new List<RoomAllocation>();
                int i = 0;
                foreach (var room in roomsAllocation)
                {
                    var allocation = new RoomAllocation();
                    allocation.Adults = room.Adults + room.Teenagers;
                    allocation.Children = room.Children;
                    allocation.Infants = room.Infants;
                    allocation.ChildAges = room.ChildAges;
                    allocation.Rooms = new List<RoomModel>();
                    allocation.Rooms.Add(reservedRooms.ElementAt(i));
                    i++;
                }
                available = await sunApiRequest.CheckAvailabilityAsync(Guid, ResultId, allocations);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!available)
            {
                ViewBag.BookResult = "<h3>Info</h3><h5>This item not available now</h5>";
                return View("BookResult");
            }
            ViewBag.GUID = Guid;
            ViewBag.ResultId = ResultId;
            ViewBag.TotalRooms = rooms.Count;
            ViewBag.Adults = roomsAllocation.Sum(r => r.Adults);
            ViewBag.Teenagers = roomsAllocation.Sum(r => r.Teenagers);
            ViewBag.Children = roomsAllocation.Sum(r => r.Children);
            ViewBag.Infants = roomsAllocation.Sum(r => r.Infants);
            ViewBag.CountryId = new SelectList(unitOfWork.CountryRepository.Get(), "Id", "Name");
            var reservation = new ReservationModel();
            reservation.TotalAmount = totalCost;
            reservation.ReservationFrom = CheckIn;
            reservation.ReservationTo = CheckOut;
            reservation.ReservationType = HotelType;
            reservation.Currency = isUSD ? Currency.USD : Currency.EGP;
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
        public ActionResult BookTour(ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var reservation = Mapper.Map<Reservation>(reservationModel);
                reservation.OrderId = IdUtils.generateSampleId();
                reservation.ReservationType = (int)ReservationType.Tour;
                if (User.Identity.IsAuthenticated)
                {
                    reservation.UserId = User.Identity.GetUserId();
                }
                unitOfWork.ReservationRepository.Insert(reservation);
                unitOfWork.Save();
                var roles = context.Roles.Where(r => r.Name != "Customer").Select(r => r.Id);
                var emails = context.Users.Where(u => roles.Any(r => r == u.Roles.FirstOrDefault().RoleId)).Select(u => u.Email).ToList();
                NotificationHelper.NotifySuccessBooking(reservation, emails);
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

        [Route("Book/Property")]
        [HttpPost]
        public ActionResult BookProperty(ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var reservation = Mapper.Map<Reservation>(reservationModel);
                reservation.OrderId = IdUtils.generateSampleId();
                reservation.ReservationType = (int)ReservationType.Property;
                if (DateTime.Now.Year > reservation.ReservationFrom.Year)
                {
                    reservation.ReservationFrom = DateTime.Now;
                    reservation.ReservationTo = DateTime.Now;
                }
                if (User.Identity.IsAuthenticated)
                {
                    reservation.UserId = User.Identity.GetUserId();
                }
                unitOfWork.ReservationRepository.Insert(reservation);
                unitOfWork.Save();
                var roles = context.Roles.Where(r => r.Name != "Customer").Select(r => r.Id);
                var emails = context.Users.Where(u => roles.Any(r => r == u.Roles.FirstOrDefault().RoleId)).Select(u => u.Email).ToList();
                NotificationHelper.NotifySuccessBooking(reservation, emails);
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
        public async Task<ActionResult> BookHotel(string GUID, string ResultId, ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var reservation = Mapper.Map<Reservation>(reservationModel);
                if(reservationModel.ReservationType == ReservationType.SunHotel)
                {
                    var country = unitOfWork.CountryRepository.GetByID(reservationModel.CountryId);
                    reservationModel.CountryCode = country.Code;
                    var bookRef = await sunApiRequest.BookHotelAsync(GUID, ResultId, reservationModel);
                    if (string.IsNullOrEmpty(bookRef))
                    {
                        return View("BookResult");
                    }
                    reservation.OrderId = bookRef;
                }
                else
                {
                    reservation.ItemId = int.Parse(ResultId);
                    reservation.OrderId = IdUtils.generateSampleId();
                    var rooms = (List<RoomModel>)Session["ReservedRooms"];
                    foreach (var item in  rooms)
                    {
                        var room = unitOfWork.RoomRepository.GetByID(item.Id);
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
                var roles = context.Roles.Where(r => r.Name != "Customer").Select(r => r.Id);
                var emails = context.Users.Where(u => roles.Any(r => r == u.Roles.FirstOrDefault().RoleId)).Select(u => u.Email).ToList();
                NotificationHelper.NotifySuccessBooking(reservation, emails);
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
                DateTime checkIn = new DateTime(2020, 4, 23);
                DateTime checkOut = new DateTime(2020, 4, 30);
                string data = "<SearchRequest>" +
                                    "<LocationID>5918</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>2</Adults>" +
                                            "<Children>0</Children>" +
                                            "<Infants>0</Infants>" +
                                        "</RoomAllocation>" +
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
                        var isComplete = xmlResult.Root.Element("IsComplete");
                        if (isSuccess.Value == "false" || (isComplete.Value == "true" && isSuccess.Value == "true"))
                        {
                            return Content(result, "application/xml");
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                    }
                    else
                    {
                        return Content("Error");
                    }
                }
            }
        }

        [Route("api2")]
        public async Task<ActionResult> TestAsync(string Guid, string ResultId, string RoomId, string RateId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Availability");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<AvailabilityRequest>" +
                                    "<GUID>" + Guid + "</GUID>" +
                                    "<Stage>Start</Stage>" +
                                    "<ResultID>" + ResultId + "</ResultID>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>2</Adults>" +
                                            "<Children>0</Children>" +
                                            "<Infants>0</Infants>" +
                                            "<RoomID>" + RoomId + "</RoomID>" +
                                            "<RoomRateID>" + RateId + "</RoomRateID>" +
                                        "</RoomAllocation>" +
                                    "</RoomAllocations>" +
                                "</AvailabilityRequest>";
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
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Element("Stage").Value = "Ping";
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                    }
                    else
                    {
                        return Content("Error");
                    }
                }
            }
        }

        [Route("api3")]
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
                                    "<Passengers>" +
                                        "<Passenger>" +
                                            "<Title>Mr</Title>" +
                                            "<Firstname>Ahmed</Firstname>" +
                                            "<Surname>Ayman</Surname>" +
                                            "<Type>Adult</Type>" +
                                            "<DOB>" + dob.ToString("yyyy-MM-dd") + "</DOB>" +
                                        "</Passenger>" +
                                         "<Passenger>" +
                                            "<Title>Mr</Title>" +
                                            "<Firstname>Ahmed2</Firstname>" +
                                            "<Surname>Ayman2</Surname>" +
                                            "<Type>Adult</Type>" +
                                            "<DOB>" + dob.ToString("yyyy-MM-dd") + "</DOB>" +
                                        "</Passenger>" +
                                    "</Passengers>" +
                                    "<Reference>MYREFERENCE</Reference>" +
                                    "<CustomerTitle>Mr</CustomerTitle>" +
                                    "<CustomerFName>Ahmed</CustomerFName>" +
                                    "<CustomerSName>Ayman</CustomerSName>" +
                                    "<CustomerAddress1>Test Address 1</CustomerAddress1>" +
                                    "<CustomerAddress2>Test Address 2</CustomerAddress2>" +
                                    "<CustomerCity>Cairo</CustomerCity>" +
                                    "<CustomerPostCode>55</CustomerPostCode>" +
                                    "<CustomerCountryCode>EG</CustomerCountryCode>" +
                                    "<CustomerTelDay>0122455</CustomerTelDay>" +
                                    "<CustomerTelEve>1545454</CustomerTelEve>" +
                                    "<CustomerEmail>ahmed@gmail.com</CustomerEmail>" +
                                "</BookRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                HttpResponseMessage Res = await client.PostAsync("", content);
                if (Res.IsSuccessStatusCode)
                {
                    var result = Res.Content.ReadAsStringAsync().Result;
                    return Content(result, "application/xml");
                }
                else
                {
                    return Content("Error");
                }
            }
        }
    }
}