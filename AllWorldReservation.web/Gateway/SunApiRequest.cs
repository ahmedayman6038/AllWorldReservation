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

        public async Task<IEnumerable<HotelModel>> SearchHotelAsync(string code, DateTime checkIn, DateTime checkOut, List<SearchViewModel> roomsSearch)
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
                                var rooms = item.Element("RoomAllocations").Element("RoomAllocation").Element("Rooms").Elements();
                                var prices = new List<float>();
                                foreach (var room in rooms)
                                {
                                    prices.Add(float.Parse(room.Element("TotalAmount").Value));
                                }
                                hotel.Price = prices.Min();
                                hotel.Stars = int.Parse(item.Element("ClassCode").Value.Replace("*", string.Empty));
                                hotel.Type = BL.Enums.EnumCollection.ReservationType.SunHotel;
                                hotels.Add(hotel);
                            }
                            return hotels;
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return hotels;
                    }
                }
            }
        }

        public async Task<HotelModel> GetHotelAsync(string code, DateTime checkIn, DateTime checkOut, List<SearchViewModel> roomsSearch, string itemId)
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
                                    var hotel = new HotelModel();
                                    hotel.Id = int.Parse(resultId.Replace(".", string.Empty));
                                    hotel.ResultId = resultId;
                                    hotel.GUID = GUID;
                                    hotel.Name = item.Element("Name").Value;
                                    hotel.Address = item.Element("Address").Value;
                                    hotel.Place = new PlaceModel() { Name = item.Element("Location").Value };
                                    hotel.Description = item.Element("Description").Value;
                                    var roomsElments = item.Element("RoomAllocations").Element("RoomAllocation").Element("Rooms").Elements();
                                    var rooms = new List<RoomModel>();
                                    var minPrice = float.MaxValue;
                                    foreach (var element in roomsElments)
                                    {
                                        var room = new RoomModel();
                                        room.Id = int.Parse(element.Element("RoomID").Value);
                                        //room.RateId = int.Parse(element.Element("RoomRateID").Value);
                                        room.Name = element.Element("Name").Value;
                                        room.Description = element.Element("Description").Value;
                                        //room.BoardBasis = element.Element("BoardBasis").Value;
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
                                    return hotel;
                                }
                            }
                            return null;
                        }
                        var guid = xmlResult.Root.Element("GUID");
                        var xmlRequest = XDocument.Parse(data);
                        xmlRequest.Root.Add(guid);
                        content = new StringContent(xmlRequest.ToString(), Encoding.UTF8, "application/xml");
                        Thread.Sleep(1000);
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
                        return String.Empty;
                    }
                }
                else
                {
                    return String.Empty;
                }
            }
        }

    }
}