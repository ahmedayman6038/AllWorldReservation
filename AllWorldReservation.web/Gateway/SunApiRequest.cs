using AllWorldReservation.BL.Models;
using AllWorldReservation.web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using static AllWorldReservation.BL.Enums.EnumCollection;

namespace AllWorldReservation.web.Gateway
{
    public class SunApiRequest
    {
        private string Baseurl;
        private string AuthCode;

        public SunApiRequest()
        {
            Baseurl = ConfigurationManager.AppSettings["SUN_API_BASE_URL"];
            AuthCode = ConfigurationManager.AppSettings["SUN_API_AUTHCODE"];
        }

        public async Task<IEnumerable<HotelModel>> SearchHotelAsync(string code, DateTime checkIn, DateTime checkOut, List<GuestsViewModel> roomsSearch)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<SearchRequest>" +
                                    "<LocationID>" + code + "</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                    "<RoomAllocations>";
                                    foreach (var room in roomsSearch)
                                    {
                                        data += "<RoomAllocation>" +
                                              "<Adults>" + room.Adults + "</Adults>" +
                                              "<Children>" + room.Children + "</Children>" +
                                              "<Infants>" + room.Infants + "</Infants>";
                                            if(room.ChildAges != null)
                                            {
                                                data += "<ChildAges>";
                                                foreach (var age in room.ChildAges)
                                                {
                                                    data += "<int>" + age + "</int>";
                                                }
                                                data += "</ChildAges>";
                                            }                                                                                                                             
                                            data += "</RoomAllocation>";
                                    }
                                  
                                   data += "</RoomAllocations>" +
                                "</SearchRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var hotels = new List<HotelModel>();
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
                            return hotels;
                        }
                        else if (isComplete.Value == "true" && isSuccess.Value == "true")
                        {
                            var results = xmlResult.Root.Element("Results");
                            var GUID = xmlResult.Root.Element("GUID").Value;
                            foreach (var item in results.Elements())
                            {
                                var hotel = new HotelModel();
                                var resultId = item.Element("ID").Value;
                                hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
                                hotel.ResultId = resultId;
                                hotel.GUID = GUID;
                                hotel.Name = item.Element("Name").Value;
                                hotel.Address = item.Element("Address").Value;
                                hotel.Place = new PlaceModel() { Name = item.Element("Location").Value };
                                hotel.Description = item.Element("Description").Value.Substring(0, 150) + "...";
                                var roomAllocations = item.Element("RoomAllocations").Elements();
                                var minPrice = float.MaxValue;
                                foreach (var roomAllocation in roomAllocations)
                                {
                                    var rooms = roomAllocation.Element("Rooms").Elements();
                                    foreach (var room in rooms)
                                    {
                                        var totalAmount = float.Parse(room.Element("TotalAmount").Value);
                                        if (totalAmount < minPrice)
                                        {
                                            minPrice = totalAmount;
                                        }
                                    }
                                }
                                hotel.PriceFromUSD = minPrice;
                                hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*", string.Empty));
                                hotel.Type = ReservationType.SunHotel;
                                hotels.Add(hotel);
                            }
                            return hotels;
                        }
                        else
                        {
                            var guid = xmlResult.Root.Element("GUID");
                            var xmlRequest = XDocument.Parse(data);
                            xmlRequest.Root.Add(guid);
                            content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        }
                    }
                    else
                    {
                        return hotels;
                    }
                }
            }
        }

        public async Task<HotelViewModel> GetHotelAsync(string code, DateTime checkIn, DateTime checkOut, List<GuestsViewModel> roomsSearch, string itemId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Search");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<SearchRequest>" +
                                    "<LocationID>" + code + "</LocationID>" +
                                    "<CheckIn>" + checkIn.ToString("yyyy-MM-dd") + "</CheckIn>" +
                                    "<CheckOut>" + checkOut.ToString("yyyy-MM-dd") + "</CheckOut>" +
                                      "<RoomAllocations>";
                                            foreach (var room in roomsSearch)
                                            {
                                                data += "<RoomAllocation>" +
                                                      "<Adults>" + room.Adults + "</Adults>" +
                                                      "<Children>" + room.Children + "</Children>" +
                                                      "<Infants>" + room.Infants + "</Infants>";
                                                if (room.ChildAges != null)
                                                {
                                                    data += "<ChildAges>";
                                                    foreach (var age in room.ChildAges)
                                                    {
                                                        data += "<int>" + age + "</int>";
                                                    }
                                                    data += "</ChildAges>";
                                                }
                                                data += "</RoomAllocation>";
                                            }
                                            data += "</RoomAllocations>" +
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
                        if (isSuccess.Value == "false")
                        {
                            return null;
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
                                    var hotel = new HotelViewModel();
                                    hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
                                    hotel.ResultId = resultId;
                                    hotel.GUID = GUID;
                                    hotel.Name = item.Element("Name").Value;
                                    hotel.Address = item.Element("Address").Value;
                                    hotel.Place = item.Element("Location").Value;
                                    hotel.Description = item.Element("Description").Value;
                                    var allocaionElements = item.Element("RoomAllocations").Elements();
                                    var roomAllocations = new List<RoomAllocation>();
                                    var minPrice = float.MaxValue;
                                    foreach (var allocaionElement in allocaionElements)
                                    {
                                        var roomAllocation = new RoomAllocation();
                                        roomAllocation.Id = int.Parse(allocaionElement.Element("Idx").Value);
                                        roomAllocation.Adults = int.Parse(allocaionElement.Element("Adults").Value);
                                        roomAllocation.Children = int.Parse(allocaionElement.Element("Children").Value);
                                        roomAllocation.Infants = int.Parse(allocaionElement.Element("Infants").Value);
                                        var roomElements = allocaionElement.Element("Rooms").Elements();
                                        var rooms = new List<RoomModel>();
                                        foreach (var roomElement in roomElements)
                                        {
                                            var room = new RoomModel();
                                            room.Id = int.Parse(roomElement.Element("RoomID").Value);
                                            room.RateId = int.Parse(roomElement.Element("RoomRateID").Value);
                                            room.Name = roomElement.Element("Name").Value;
                                            room.Description = roomElement.Element("Description").Value;
                                            room.PriceUSD = float.Parse(roomElement.Element("TotalAmount").Value);
                                            if (room.PriceUSD < minPrice)
                                            {
                                                minPrice = room.PriceUSD;
                                            }
                                            rooms.Add(room);
                                        }
                                        roomAllocation.Rooms = rooms;
                                        roomAllocations.Add(roomAllocation);
                                    }
                                    hotel.PriceFromUSD = minPrice;
                                    hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*", string.Empty));
                                    hotel.RoomAllocations = roomAllocations;
                                    hotel.Type = ReservationType.SunHotel;
                                    return hotel;
                                }
                            }
                            return null;
                        }
                        else
                        {
                            var guid = xmlResult.Root.Element("GUID");
                            var xmlRequest = XDocument.Parse(data);
                            xmlRequest.Root.Add(guid);
                            content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async Task<string> BookHotelAsync(string GUID, string ResultId, ReservationModel reservationModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("AuthCode", AuthCode);
                client.DefaultRequestHeaders.Add("Action", "Book");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                string data = "<BookRequest>" +
                                    "<GUID>" + GUID + "</GUID>" +
                                    "<ResultID>" + ResultId + "</ResultID>" +
                                    "<Passengers>";
                foreach (var guest in reservationModel.Guests)
                {
                    data += "<Passenger>" +
                                "<Title>" + guest.Title + "</Title>" +
                                "<Firstname>" + guest.FirstName + "</Firstname>" +
                                "<Surname>" + guest.Surname + "</Surname>" +
                                "<Type>Adult</Type>" +
                                "<DOB>" + guest.DateOfBirth.ToString("yyyy-MM-dd") + "</DOB>" +
                           "</Passenger>";
                }
                data += "</Passengers>" +
                        "<Reference>" + reservationModel.OrderId + "</Reference>" +
                        "<CustomerTitle>Mr</CustomerTitle>" +
                        "<CustomerFName>Ahmed</CustomerFName>" +
                        "<CustomerSName>Ayman</CustomerSName>" +
                        "<CustomerAddress1>" + reservationModel.Address1 + "</CustomerAddress1>" +
                        "<CustomerAddress2>" + reservationModel.Address2 + "</CustomerAddress2>" +
                        "<CustomerCity>" + reservationModel.City + "</CustomerCity>" +
                        "<CustomerPostCode>" + reservationModel.PostCode + "</CustomerPostCode>" +
                        "<CustomerCountryCode>" + reservationModel.CountryCode + "</CustomerCountryCode>" +
                        "<CustomerTelDay>" + reservationModel.TelNo1 + "</CustomerTelDay>" +
                        "<CustomerTelEve>" + reservationModel.TelNo2 + "</CustomerTelEve>" +
                        "<CustomerEmail>" + reservationModel.Email + "</CustomerEmail>" +
                        "</BookRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                HttpResponseMessage Res = await client.PostAsync("", content);
                if (Res.IsSuccessStatusCode)
                {
                    var result = Res.Content.ReadAsStringAsync().Result;
                    var xmlResult = XDocument.Parse(result);
                    var isSuccess = xmlResult.Root.Element("IsSuccess");
                    var isComplete = xmlResult.Root.Element("IsComplete");
                    if (isComplete.Value == "true" && isSuccess.Value == "true")
                    {
                        var bookRef = xmlResult.Root.Element("BookingRef");
                        return bookRef.Value;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public async Task<bool> CheckAvailabilityAsync(string Guid, string ResultId, List<RoomAllocation> roomsAllocation)
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
                                    "<RoomAllocations>";
                                    foreach (var room in roomsAllocation)
                                    {
                                        data += "<RoomAllocation>" +
                                                    "<Adults>" + room.Adults + "</Adults>" +
                                                    "<Children>" + room.Children + "</Children>" +
                                                    "<Infants>" + room.Infants + "</Infants>";
                                                    if (room.ChildAges != null)
                                                    {
                                                        data += "<ChildAges>";
                                                        foreach (var age in room.ChildAges)
                                                        {
                                                            data += "<int>" + age + "</int>";
                                                        }
                                                        data += "</ChildAges>";
                                                    }
                                           data += "<RoomID>" + room.Rooms.First().Id + "</RoomID>" +
                                                   "<RoomRateID>" + room.Rooms.First().RateId + "</RoomRateID>" +
                                             "</RoomAllocation>";
                                    }                           
                            data += "</RoomAllocations>" +
                           "</AvailabilityRequest>";
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var available = false;
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
                            return available;
                        }
                        else if (isComplete.Value == "true" && isSuccess.Value == "true")
                        {
                            var element = xmlResult.Root.Element("Results").Element("AccommodationResult").Element("IsAvailable");
                            if(element.Value == "true")
                            {
                                available = true;
                            }
                            return available;
                        }
                        else
                        {
                            var xmlRequest = XDocument.Parse(data);
                            xmlRequest.Root.Element("Stage").Value = "Ping";
                            content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        }
                    }
                    else
                    {
                        return available;
                    }
                }
            }
        }


    }
}