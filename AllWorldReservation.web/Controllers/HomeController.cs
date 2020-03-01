using AllWorldReservation.BL.Models;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
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
        string AuthCode = "test|test12345";
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
                client.BaseAddress = new Uri(Baseurl);                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                DateTime checkIn = new DateTime(2020,3,5);
                DateTime checkOut = new DateTime(2020,3,15);
                string data = "<SearchRequest>" +
                                    "<LocationID>17320</LocationID>" +
                                    "<CheckIn>"+checkIn.ToString("yyyy-MM-dd")+"</CheckIn>" +
                                    "<CheckOut>"+checkOut.ToString("yyyy-MM-dd")+"</CheckOut>" +
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


        [Route("Search/Hotel")]
        public async Task<ActionResult> SearchHotel(int destination, DateTime checkIn, DateTime checkOut, int guest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<SearchRequest>" +
                                    "<LocationID>"+destination+"</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>"+guest+"</Adults>" +
                                        "</RoomAllocation>" +
                                    "</RoomAllocations>" +
                                "</SearchRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var hotels = new List<HotelModel>();
                var location = "Hotel";
                while (true)
                {
                    HttpResponseMessage Res = await client.PostAsync("", content);
                    if (Res.IsSuccessStatusCode)
                    {
                        var result = Res.Content.ReadAsStringAsync().Result;
                        var xmlResult = XDocument.Parse(result);
                        var isSuccess = xmlResult.Root.Element("IsSuccess");
                        var isComplete = xmlResult.Root.Element("IsComplete");
                        if (isSuccess.Value == "false")
                        {
                            ViewBag.Location = location;
                            return View(hotels);
                        }
                        else if(isComplete.Value == "true" && isSuccess.Value == "true")
                        {
                            var results = xmlResult.Root.Element("Results");
                            var GUID = xmlResult.Root.Element("GUID").Value;
                            foreach (var item in results.Elements())
                            {
                                var hotel = new HotelModel();
                                var resultId = item.Element("ID").Value;
                                hotel.Id = int.Parse(resultId.Replace(".",string.Empty));
                                hotel.ResultId = resultId;
                                hotel.GUID = GUID;
                                hotel.Name = item.Element("Name").Value;
                                hotel.Location = item.Element("Address").Value;
                                location = item.Element("Location").Value;
                                hotel.Description = item.Element("Description").Value.Substring(0,150) + "...";
                                var rooms = item.Element("RoomAllocations").Element("RoomAllocation").Element("Rooms").Elements();
                                var prices = new List<float>();
                                foreach (var room in rooms)
                                {
                                    prices.Add(float.Parse(room.Element("TotalAmount").Value));
                                }
                                hotel.Price = prices.Min();
                                hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*",string.Empty));
                                hotels.Add(hotel);
                            }
                            ViewBag.Adults = guest;
                            ViewBag.CheckIn = checkIn;
                            ViewBag.CheckOut = checkOut;
                            ViewBag.Destination = destination;
                            ViewBag.Location = location;
                            return View(hotels);
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        ViewBag.Location = location;
                        return View(hotels);
                    }
                }
            }
        }

        [Route("Check/Hotel")]
        public async Task<ActionResult> CheckHotel(int destination, DateTime checkIn, DateTime checkOut, int guest, string itemId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<SearchRequest>" +
                                    "<LocationID>" + destination + "</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                    "<RoomAllocations>" +
                                        "<RoomAllocation>" +
                                            "<Adults>" + guest + "</Adults>" +
                                        "</RoomAllocation>" +
                                    "</RoomAllocations>" +
                                "</SearchRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var hotel = new HotelModel();
                var location = "Hotel";
                while (true)
                {
                    HttpResponseMessage Res = await client.PostAsync("", content);
                    if (Res.IsSuccessStatusCode)
                    {
                        var result = Res.Content.ReadAsStringAsync().Result;
                        var xmlResult = XDocument.Parse(result);
                        var isSuccess = xmlResult.Root.Element("IsSuccess");
                        var isComplete = xmlResult.Root.Element("IsComplete");
                        if (isSuccess.Value == "false")
                        {
                            ViewBag.Location = location;
                            return View(hotel);
                        }
                        else if (isComplete.Value == "true" && isSuccess.Value == "true")
                        {
                            var results = xmlResult.Root.Element("Results");
                            var GUID = xmlResult.Root.Element("GUID").Value;
                            foreach (var item in results.Elements())
                            {
                                var resultId = item.Element("ID").Value;
                                if (resultId == itemId)
                                {
                                    hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
                                    hotel.ResultId = resultId;
                                    hotel.GUID = GUID;
                                    hotel.Name = item.Element("Name").Value;
                                    hotel.Location = item.Element("Address").Value;
                                    location = item.Element("Location").Value;
                                    hotel.Description = item.Element("Description").Value;
                                    var roomsElments = item.Element("RoomAllocations").Element("RoomAllocation").Element("Rooms").Elements();
                                    var rooms = new List<RoomModel>();
                                    var minPrice = float.MaxValue;
                                    foreach (var element in roomsElments)
                                    {
                                        var room = new RoomModel();
                                        room.Id = int.Parse(element.Element("RoomID").Value);
                                        room.RateId = int.Parse(element.Element("RoomRateID").Value);
                                        room.Name = element.Element("Name").Value;
                                        room.Description = element.Element("Description").Value;
                                        room.BoardBasis = element.Element("BoardBasis").Value;
                                        room.TotalAmount = float.Parse(element.Element("TotalAmount").Value);
                                        if (room.TotalAmount < minPrice)
                                        {
                                            minPrice = room.TotalAmount;
                                        }
                                        rooms.Add(room);
                                    }
                                    hotel.Price = minPrice;
                                    hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*", string.Empty));
                                    hotel.Rooms = rooms;
                                    ViewBag.Guest = guest;
                                    ViewBag.Location = location;
                                    return View(hotel);
                                }                              
                            }
                            ViewBag.Location = location;
                            return View(hotel);
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        ViewBag.Location = location;
                        return View(hotel);
                    }
                }
            }
        }

        [Route("Book/Hotel")]
        [HttpGet]
        public ActionResult BookHotel(string Guid, string ResultId, int Guest)
        {
            ViewBag.GUID = Guid;
            ViewBag.ResultId = ResultId;
            ViewBag.Guest = Guest;
            return View();
        }

        [Route("Book/Hotel")]
        [HttpPost]
        public async Task<ActionResult> BookHotel(BookModel bookModel)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                    client.DefaultRequestHeaders.Add("Action", "Search");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    string data = "<BookRequest>" +
                                        "<GUID>" + bookModel.GUID + "</GUID>" +
                                        "<ResultID>" + bookModel.ResultId + "</ResultID>" +
                                        "<Passengers>";
                    foreach (var guest in bookModel.Guests)
                    {
                        data += "<Passenger>" +
                                    "<Title>" + guest.Title + "</Title>" +
                                    "<Firstname>" + guest.FirstName + "</Firstname>" +
                                    "<Surname>" + guest.Surname + "</Surname>" +
                                    "<Type>Adult</Type>" +
                                    "<DOB>" + guest.DateOfBirth.ToString("yyyy-MM-dd") + " </DOB>" +
                               "</Passenger>";
                    }
                    data += "</Passengers>" +
                            "<Reference>MYREFERENCE</Reference>"+
                            "<CustomerTitle>" +bookModel.Guests.First().Title+"</CustomerTitle>"+
                            "<CustomerFName>" + bookModel.Guests.First().FirstName + "</CustomerFName>" +
                            "<CustomerSName>" + bookModel.Guests.First().Surname + "</CustomerSName>" +
                            "<CustomerAddress1>" + bookModel.Address1 + "</CustomerAdress1>" +
                            "<CustomerAddress2>" + bookModel.Address2 + "</CustomerAdress2>" +
                            "<CustomerCity>" + bookModel.City + "</CustomerCity>" +
                            "<CustomerPostCode>" + bookModel.PostCode + "</CustomerPostCode>" +
                            "<CustomerCountryCode>" + bookModel.Country + "</CustomerCountryCode>" +
                            "<CustomerTelDay>" + bookModel.TelNo1 + "</CustomerTelDay>" +
                            "<CustomerTelEve>" + bookModel.TelNo2 + "</CustomerTelEve>" +
                            "<CustomerEmail>" + bookModel.Email + "</CustomerEmail>" +
                            "</BookRequest>";
                    var content = new StringContent(data, Encoding.UTF8, "application/xml");
                    //HttpResponseMessage Res = await client.PostAsync("", content);
                    //if (Res.IsSuccessStatusCode)
                    //{
                    //    var result = Res.Content.ReadAsStringAsync().Result;
                    //    var xmlResult = XDocument.Parse(result);
                    //    var isSuccess = xmlResult.Root.Element("IsSuccess");
                    //    var isComplete = xmlResult.Root.Element("IsComplete");
                    //    if (isSuccess.Value == "false" || (isComplete.Value == "true" && isSuccess.Value == "true"))
                    //    {
                    //        return Content(result, "application/xml");
                    //    }
                    //    var guid = xmlResult.Root.Element("GUID");
                    //    var xmlRequest = XDocument.Parse(data);
                    //    xmlRequest.Root.Add(guid);
                    //    content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                    //    Thread.Sleep(1000);
                    //}
                    //else
                    //{
                    //    return Content("Error");
                    //}
                }
            }
            ViewBag.GUID = bookModel.GUID;
            ViewBag.ResultId = bookModel.ResultId;
            ViewBag.Guest = bookModel.Guests.Count();
            return View();
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