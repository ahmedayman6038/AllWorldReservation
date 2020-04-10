using AllWorldReservation.web.Gateway;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using AllWorldReservation.BL.Repositories;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.web.Helper;

namespace AllWorldReservation.web.Controllers
{
    /// <summary>
    /// Abstract controller to group the common methods
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly GatewayApiConfig GatewayApiConfig;
      //  protected readonly ILogger Logger;
        protected readonly GatewayApiClient GatewayApiClient;
        protected readonly NVPApiClient NVPApiClient;
        protected readonly Boolean isOSPlatformWindows;
        private ApplicationDbContext context = new ApplicationDbContext();
        private UnitOfWork unitOfWork;
        //workaround for session issue on MacOS and Linux
        private static Dictionary<string, string> FakeSession = new Dictionary<string, string>();

        protected Dictionary<string, string> ViewList = new Dictionary<string, string>();


        protected BaseController()
        {
            GatewayApiConfig = new GatewayApiConfig();
            GatewayApiClient = new GatewayApiClient(GatewayApiConfig);
            NVPApiClient = new NVPApiClient();
            unitOfWork = new UnitOfWork(context);
            isOSPlatformWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            initViewList();
        }

        private void initViewList()
        {
            ViewList.Add("ApiResponse", "~/Views/Payment/ApiResponse.cshtml");
            ViewList.Add("Receipt", "~/Views/Payment/Receipt.cshtml");
            ViewList.Add("SecureIdPayerAuthenticationForm", "~/Views/Payment/SecureIdPayerAuthenticationForm.cshtml");
            ViewList.Add("MasterpassButton", "~/Views/Payment/MasterpassButton.cshtml");
        }


        /// <summary>
        /// Builds the view data for NVP method.
        /// </summary>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        /// <param name="response">Response.</param>
        protected void buildViewDataNVP(GatewayApiRequest gatewayApiRequest, string response)
        {

            ViewBag.Operation = gatewayApiRequest.ApiOperation;
            ViewBag.Method = gatewayApiRequest.ApiMethod;
            ViewBag.RequestUrl = gatewayApiRequest.RequestUrl;

            StringBuilder sb = new StringBuilder();

            //remove credentials from parameters before display
            gatewayApiRequest.NVPParameters.Remove("apiUsername");
            gatewayApiRequest.NVPParameters.Remove("apiPassword");
            foreach (var param in gatewayApiRequest.NVPParameters)
            {
                sb.Append(sb.Length > 0 ? ", " : "");
                sb.AppendFormat("{0}={1}", param.Key, param.Value);
            }
            ViewBag.Payload = sb.ToString();

            //split result and add one information per line
            sb = new StringBuilder();
            foreach (var param in response.Split('&'))
            {
                sb.AppendLine(param);
            }
            ViewBag.Response = sb.ToString();
        }


        /// <summary>
        /// Builds the default view data 
        /// </summary>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        /// <param name="response">Response.</param>
        protected void buildViewData(GatewayApiRequest gatewayApiRequest, string response, int ReservationId)
        {
            JObject jObject = JsonHelper.getJObject(response);
            string result = jObject["result"].ToString();
            string orderId = "", transId = "";
            if(result == "SUCCESS")
            {
                orderId = jObject["order"]["id"].ToString();
                transId = jObject["transaction"]["id"].ToString();
                var reservation = unitOfWork.ReservationRepository.GetByID(ReservationId);
                if (orderId == reservation.OrderId)
                {
                    reservation.Paied = true;
                    unitOfWork.ReservationRepository.Update(reservation);
                    unitOfWork.Save();
                    ViewBag.PaymentResult = "<h3>Success</h3><h5>Your Payment Received Successfuly</h5><h5>Your Transaction: " + transId + " </h5>";
                    NotificationHelper.NotifySuccessPayment(reservation);
                    return;
                }
            }
            ViewBag.PaymentResult = "<h3>Error</h3><h5>Your Payment Not Received Please try later</h5>";
        }

        //Session operations
        protected void setSessionValue(String key, String value)
        {
            removeSessionValue(key);
            if (isOSPlatformWindows)
            {
                this.HttpContext.Session[key] = value;
            }
            else
            {
                FakeSession.Add(key, value);
            }
        }


        protected String getSessionValueAsString(String key)
        {
            String value;
            if (isOSPlatformWindows)
            {
                value = this.HttpContext.Session[key].ToString();
            }
            else
            {
                if (FakeSession.ContainsKey(key))
                {
                    value = FakeSession[key];
                }
                else
                {
                    value = null;
                }
            }
            return value;
        }

        protected void removeSessionValue(String key)
        {
            if (isOSPlatformWindows)
            {
                if (this.HttpContext.Session.Keys.ToString().Contains(key))
                {
                    this.HttpContext.Session.Remove(key);
                }
            }
            else
            {
                if (FakeSession.ContainsKey(key))
                {
                    FakeSession.Remove(key);
                }
            }
        }

        /// <summary>
        /// Returns default request id, based on the Activity if it exists or
        /// the trace identifier from http context
        /// </summary>
        /// <returns>The request identifier.</returns>
        protected string getRequestId()
        {
            return Activity.Current?.Id ?? HttpContext.Trace.ToString();
        }
    }
}