using AllWorldReservation.BL.Repositories;
using AllWorldReservation.BL.Utils;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.web.Gateway;
using AllWorldReservation.web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllWorldReservation.web.Controllers
{
    public class PaymentController : BaseController
    {
        private DbContainer context = new DbContainer();
        private UnitOfWork unitOfWork;

        public PaymentController()
        {
            unitOfWork = new UnitOfWork(context);
        }
        /// <summary>
        /// Display Index Page.
        /// </summary>
        /// <returns>The index.</returns>
        //[HttpGet]
        //public ActionResult Index()
        //{
        //   // Logger.LogInformation("Payment controller Index action");

        //    return Redirect("Pay");
        //}

        /// <summary>
        /// Display PAY operation page, view Pay.cshtml
        /// </summary>
        [HttpGet]
        [Route("bookhotel/{id}")]
        public ActionResult ShowPayHotel(int id)
        {
            //  Logger.LogInformation("Payment controller ShowPay action");
            var hotel = unitOfWork.HotelRepository.GetByID(id);
            if(hotel == null)
            {
                return HttpNotFound();
            }
            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);       
            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = getTestAndGoLiveDocumentationURL();
            ViewBag.Hotel = hotel;
            return View("PayHotel", gatewayApiRequest);
        }

        [HttpGet]
        [Route("bookplace/{id}")]
        public ActionResult ShowPayPlace(int id)
        {
            //  Logger.LogInformation("Payment controller ShowPay action");
            var place = unitOfWork.PlaceRepository.GetByID(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);
            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = getTestAndGoLiveDocumentationURL();
            ViewBag.Place = place;
            return View("PayPlace", gatewayApiRequest);
        }

        /// <summary>
        /// Display PAY operation page, view Pay.cshtml
        /// </summary>
        [HttpGet]
        [Route("payWithToken")]
        public ActionResult ShowPayWithToken()
        {
          //  Logger.LogInformation("Payment controller ShowPayWithToken action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = getTestAndGoLiveDocumentationURL();
            ViewBag.ApiDocumentation = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/pickAdditionalFunctionality/tokenization/tokenization.html";

            return View("PayWithToken", gatewayApiRequest);
        }

        /// <summary>
        /// Display AUTHORIZE operation page, view Authorize.cshtml
        /// </summary>
        [HttpGet]
        [Route("authorize")]
        public ActionResult ShowAuthorize()
        {
           // Logger.LogInformation("Payment controller ShowAuthorize action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = getTestAndGoLiveDocumentationURL();

            return View("Authorize", gatewayApiRequest);
        }

        /// <summary>
        /// Display CAPTURE operation page, view Capture.cshtml
        /// </summary>
        [HttpGet]
        [Route("capture")]
        public ActionResult ShowCapture()
        {
          //  Logger.LogInformation("Payment controller ShowCapture action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig, "CAPTURE");

            return View("Capture", gatewayApiRequest);
        }

        /// <summary>
        /// Display REFUND operation page, view Refund.cshtml
        /// </summary>
        [HttpGet]
        [Route("refund")]
        public ActionResult ShowRefund()
        {
          //  Logger.LogInformation("Payment controller ShowRefund action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig, "REFUND");

            return View("Refund", gatewayApiRequest);
        }

        /// <summary>
        /// Display VOID operation page, view Void.cshtml
        /// </summary>
        [HttpGet]
        [Route("void")]
        public ActionResult ShowVoid()
        {
          //  Logger.LogInformation("Payment controller ShowVoid action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig, "VOID");

            return View("Void", gatewayApiRequest);
        }

        /// <summary>
        /// Display VERIFY operation page, view Verify.cshtml
        /// </summary>
        [HttpGet]
        [Route("verify")]
        public ActionResult ShowVerify()
        {
          //  Logger.LogInformation("Payment controller ShowVerify action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = getTestAndGoLiveDocumentationURL();

            return View("Verify");
        }

        /// <summary>
        /// Display RETRIEVE TRANSACTION operation page, view RetrieveTransaction.cshtml
        /// </summary>
        [HttpGet]
        [Route("retrieveOrder")]
        public ActionResult ShowRetrieveOrder()
        {
          //  Logger.LogInformation("Payment controller ShowRetrieveTransaction action");

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig, "RETRIEVE_ORDER");

            return View("RetrieveOrder", gatewayApiRequest);
        }

        /// <summary>
        /// Display HOSTED CHECKOUT operation page, view HostedCheckout.cshtml
        /// </summary>
        [HttpGet]
        [Route("hostedCheckout")]
        public ActionResult ShowHostedCheckout()
        {
          //  Logger.LogInformation("Payment controller HostedCheckout action");

            GatewayApiRequest gatewayApiRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayApiRequest.ApiOperation = "CREATE_CHECKOUT_SESSION";
            gatewayApiRequest.OrderId = IdUtils.generateSampleId();
            gatewayApiRequest.OrderCurrency = GatewayApiConfig.Currency;

            gatewayApiRequest.buildSessionRequestUrl();
            gatewayApiRequest.buildPayload();

            gatewayApiRequest.ApiMethod = GatewayApiClient.POST;

            try
            {
                String response = GatewayApiClient.SendTransaction(gatewayApiRequest);

             //   Logger.LogInformation("HostedCheckout response -- " + response);

                CheckoutSessionModel checkoutSessionModel = CheckoutSessionModel.toCheckoutSessionModel(response);

                ViewBag.CheckoutJsUrl = $@"{GatewayApiConfig.GatewayUrl}/checkout/version/{GatewayApiConfig.Version}/checkout.js";
                ViewBag.MerchantId = GatewayApiConfig.MerchantId;
                ViewBag.OrderId = gatewayApiRequest.OrderId;
                ViewBag.CheckoutSession = checkoutSessionModel;
                ViewBag.Currency = GatewayApiConfig.Currency;
            }
            catch (Exception e)
            {
              //  Logger.LogError($"Hosted Checkout error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }

            return View("HostedCheckout");
        }

        /// <summary>
        /// Display PAY with 3D Secure operation page, view PayWith3Ds.cshtml
        /// </summary>
        [HttpGet]
        [Route("payWith3ds")]
        public ActionResult ShowPayWith3ds()
        {
          //  Logger.LogInformation("Payment controller ShowPayWith3ds action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);

            //documentation links
            ViewBag.DocumentationCreateSessionUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Session%3a%20Create%20Checkout%20Session.html";
            ViewBag.Documentation3dsEnrollmentUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/3DS%3a%20%20Check%203DS%20Enrollment.html";
            ViewBag.Documentation3dsProcessResultUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/3DS%3a%20Process%20ACS%20Result.html";
            ViewBag.DocumentationTransactionPayUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Transaction%3a%20%20Pay.html";
            ViewBag.DocumentationTransactionAuthorizeUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Transaction%3a%20%20Authorize.html";

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            return View("PayWith3ds", gatewayApiRequest);
        }




        /// <summary>
        /// Shows the pay with paypal page.
        /// </summary>
        /// <returns>The pay with paypal view</returns>
        [HttpGet]
        [Route("payWithPayPal")]
        public ActionResult ShowPayWithPaypal()
        {
          //  Logger.LogInformation("Payment controller ShowPayWithPayPal action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.DocumentationPayPal = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/pickPaymentMethod/browserPayments/PayPal.html";

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            return View("PayWithPayPal", gatewayApiRequest);
        }




        /// <summary>
        /// Shows the pay with Union Pay page.
        /// </summary>
        /// <returns>The pay with UnionPay view</returns>
        [HttpGet]
        [Route("showPayWithUnionPaySecurePay")]
        public ActionResult ShowPayWithUnionPaySecurePay()
        {
          //  Logger.LogInformation("Payment controller ShowPayWithUnionPal action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.DocumentationUnionPay = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/pickPaymentMethod/browserPayments/testDetails.html#x_SecurePayTest";

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            return View("PayWithUnionPay", gatewayApiRequest);
        }


        /// <summary>
        /// Gets the session js URL.
        /// </summary>
        /// <returns>The session js URL.</returns>
        /// <param name="gatewayApiConfig">Gateway API config.</param>
        private string getSessionJsUrl(GatewayApiConfig gatewayApiConfig)
        {
            return $@"{GatewayApiConfig.GatewayUrl}/form/version/{GatewayApiConfig.Version}/merchant/{GatewayApiConfig.MerchantId}/session.js";
        }

        /// <summary>
        /// Shows the pay through NVP page
        /// </summary>
        /// <returns>The pay through nvp.</returns>
        [HttpGet]
        [Route("showPayThroughNVP")]
        public ActionResult ShowPayThroughNVP()
        {
           // Logger.LogInformation("Payment controller ShowPayThroughNVP action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);
            ViewBag.TestAndGoLiveUrl = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/testAndGoLive.html";

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            return View("PayThroughNVP", gatewayApiRequest);
        }


        /// <summary>
        /// Shows the Masterpass view.
        /// </summary>
        /// <returns>The masterpass.</returns>
        [HttpGet]
        [Route("showMasterpass")]
        public ActionResult ShowMasterpass()
        {
          //  Logger.LogInformation("Payment controller ShowMasterpass action");

            ViewBag.JavascriptSessionUrl = getSessionJsUrl(GatewayApiConfig);


            //documentation links
            ViewBag.MasterpassCreateSession = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Session%3a%20Create%20Session.html";
            ViewBag.MasterpassUpdateSession = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Session%3a%20Update%20Session.html";
            ViewBag.MasterpassOpenWallet = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Wallet%3a%20Open%20Wallet.html";
            ViewBag.MasterpassJSLibrary = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/pickPaymentMethod/masterPassOnline.html";
            ViewBag.MasterpassUpdateSessionFromWallet = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Wallet%3a%20Update%20Session%20From%20Wallet.html";
            ViewBag.MasterpassPay = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Transaction%3a%20%20Pay.html";
            ViewBag.MasterpassAuthorize = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/apiDocumentation/rest-json/version/latest/operation/Transaction%3a%20%20Authorize.html";
            ViewBag.MasterpassFullDocumentation = $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/pickPaymentMethod/masterPassOnline.html";


            string ORIGIN_RETURN_PAGE = "/masterpassResponse";
            string MASTERPASS_ONLINE = "MASTERPASS_ONLINE";

            GatewayApiRequest gatewayApiRequest = GatewayApiRequest.createSampleApiRequest(GatewayApiConfig);

            //build return URL
          //  string returnURL = Microsoft.AspNetCore.Http.Extensions.UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase, ORIGIN_RETURN_PAGE);

           // gatewayApiRequest.MasterpassOriginUrl = returnURL;
            gatewayApiRequest.MasterpassOnline = MASTERPASS_ONLINE;


            return View("PayWithMasterpass", gatewayApiRequest);
        }


        /// <summary>
        /// Gets the test and go live documentation URL.
        /// </summary>
        /// <returns>The test and go live.</returns>
        private string getTestAndGoLiveDocumentationURL()
        {
            return $@"{GatewayApiConfig.GatewayUrl}/api/documentation/integrationGuidelines/supportedFeatures/testAndGoLive.html";
        }


    }
}