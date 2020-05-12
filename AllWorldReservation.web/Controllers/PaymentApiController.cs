using AllWorldReservation.BL.Utils;
using AllWorldReservation.web.Gateway;
using AllWorldReservation.web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AllWorldReservation.web.Controllers
{
    /// <summary>
    /// Controller responsible to centralize the process and result methods
    /// </summary>
    /// 
    public class PaymentApiController : BaseController
    {

        public PaymentApiController()
        {
          /*  GatewayApiConfig = new GatewayApiConfig();
            GatewayApiClient = new GatewayApiClient(GatewayApiConfig);
            NVPApiClient = new NVPApiClient();*/
        }

        /// <summary>
        /// This method processes the API request for server-to-server operations. These are operations that would not commonly be invoked via a user interacting with the browser, but a system event (CAPTURE, REFUND, VOID).
        /// <summary>
        /// <param name="gatewayApiRequest">contains info on how to construct API call</param>
        /// <returns>IActionResult for api response page or error page</returns>
        [HttpPost, HttpGet]
        [Route("process")]
        public ActionResult Process(GatewayApiRequest gatewayApiRequest)
        {
          //  Logger.LogInformation($"PaymentApiController Process action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");

            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;

            //retrieve order doesnt require transaction information on the url
            if ("RETRIEVE_ORDER" == gatewayApiRequest.ApiOperation)
            {
                gatewayApiRequest.buildOrderUrl();
            }
            else
            {
                gatewayApiRequest.buildRequestUrl();
            }

            gatewayApiRequest.buildPayload();

            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

            buildViewData(gatewayApiRequest, response,0);

            return View(ViewList["ApiResponse"]);
        }

        /// <summary>
        /// This method calls the INTIATE_BROWSER_PAYMENT operation, which returns a URL to the provider's website. The user is redirected to this URL, where the purchase is completed.
        /// </summary>
        /// <param name="gatewayApiRequest">contains info on how to construct API call</param>
        /// <returns>IActionResult for api response page or error page</returns>
        [HttpPost]
        [Route("processHostedSession/{ReservationId}")]
        public ActionResult ProcessHostedSession(int? ReservationId, GatewayApiRequest gatewayApiRequest)
        {
            if(ReservationId == null)
            {
                return HttpNotFound();
            }
            //  Logger.LogInformation($"PaymentApiController ProcessHostedSession action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");
            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
            gatewayApiRequest.buildRequestUrl();
            gatewayApiRequest.buildPayload();

            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

            buildViewData(gatewayApiRequest, response, (int)ReservationId);

            return View(ViewList["ApiResponse"]);
        }

        [HttpPost]
        [Route("tokenize")]
        public ActionResult tokenize(GatewayApiRequest gatewayApiRequest)
        {
//            Logger.LogInformation($"PaymentApiController ProcessHostedSession action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");

            //update session with order details
            GatewayApiRequest gatewayUpdateSessionRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayUpdateSessionRequest.ApiMethod = GatewayApiClient.PUT;

            //update the url appending session id
            gatewayUpdateSessionRequest.buildSessionRequestUrl(gatewayApiRequest.SessionId);

            gatewayUpdateSessionRequest.OrderId = gatewayApiRequest.OrderId;
            gatewayUpdateSessionRequest.OrderCurrency = gatewayApiRequest.OrderCurrency;
            gatewayUpdateSessionRequest.OrderAmount = gatewayApiRequest.OrderAmount;

            //build payload with order info
            gatewayUpdateSessionRequest.buildPayload();

            String response = GatewayApiClient.SendTransaction(gatewayUpdateSessionRequest);

         //   Logger.LogInformation($"Tokenize updated session {response}");

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //generate token
            GatewayApiRequest gatewayGenerateTokenRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayGenerateTokenRequest.SessionId = gatewayApiRequest.SessionId;
            gatewayGenerateTokenRequest.ApiMethod = GatewayApiClient.POST;
            gatewayGenerateTokenRequest.buildPayload();
            gatewayGenerateTokenRequest.buildTokenUrl();

            response = GatewayApiClient.SendTransaction(gatewayGenerateTokenRequest);

            //validate token response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //convert json to model
            TokenResponse tokenResponse = TokenResponse.ToTokenResponse(response);

            //payment with token
            GatewayApiRequest gatewayGeneratePaymentRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayGeneratePaymentRequest.ApiOperation = "PAY";
            gatewayGeneratePaymentRequest.ApiMethod = GatewayApiClient.PUT;

            gatewayGeneratePaymentRequest.Token = tokenResponse.Token;
            gatewayGeneratePaymentRequest.SessionId = gatewayApiRequest.SessionId;
            gatewayGeneratePaymentRequest.OrderId = gatewayApiRequest.OrderId;
            gatewayGeneratePaymentRequest.TransactionId = gatewayApiRequest.TransactionId;

            gatewayGeneratePaymentRequest.buildPayload();
            gatewayGeneratePaymentRequest.buildRequestUrl();

            response = GatewayApiClient.SendTransaction(gatewayGeneratePaymentRequest);

            buildViewData(gatewayGeneratePaymentRequest, response,0);

            return View(ViewList["ApiResponse"]);
        }

        /// <summary>
        /// This method receives the callback from the Hosted Checkout redirect. It looks up the order using the RETRIEVE_ORDER operation and
        /// displays either the receipt or an error page.
        /// </summary>
        /// <param name="orderId">needed to retrieve order</param>
        /// <param name="result">Result of Hosted Checkout operation (success or error) - sent from hostedCheckout.html complete() callback</param>
        /// <returns>IActionResult for hosted checkout receipt page or error page</returns>
        [HttpGet]
        [Route("hostedCheckout/{orderId}/{result}")]
        [Route("hostedCheckout/{orderId}/{result}/{sessionId}")]
        public ActionResult HostedCheckoutReceipt(string orderId, string result, string sessionId)
        {
        //    Logger.LogInformation($"PaymentApiController HostedCheckoutReceipt action orderId {orderId} result {result} sessionId {sessionId}");

            if (result == "SUCCESS")
            {
                GatewayApiRequest gatewayApiRequest = new GatewayApiRequest(GatewayApiConfig)
                {
                    ApiOperation = "RETRIEVE_ORDER",
                    OrderId = orderId,
                    ApiMethod = GatewayApiClient.GET
                };

                gatewayApiRequest.buildOrderUrl();


                string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

              //  Logger.LogInformation($"Hosted checkout retrieve order response {response}");

                //parse response
                TransactionResponseModel transactionResponseModel = null;
                try
                {
                    transactionResponseModel = TransactionResponseModel.toTransactionResponseModel(response);
                }
                catch (Exception e)
                {
                 //   Logger.LogError($"Hosted Checkout Receipt error : {JsonConvert.SerializeObject(e)}");

                    return View("Error", new ErrorViewModel
                    {
                        RequestId = getRequestId(),
                        Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                        Message = e.Message
                    });
                }

                return View(ViewList["Receipt"], transactionResponseModel);
            }
            else
            {
             //   Logger.LogError($"The payment was unsuccessful {result}");
                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = "Payment was unsuccessful",
                    Message = "There was a problem completing your transaction."
                });
            }
        }

        /// <summary>
        /// This method handles the response from the CHECK_3DS_ENROLLMENT operation. If the card is enrolled, the response includes the HTML for the issuer's authentication form, to be injected into secureIdPayerAuthenticationForm.html.
        /// Otherwise, it displays an error.
        /// <summary> 
        /// <param name="gatewayApiRequest">needed to retrieve various data to complete API operation</param>
        /// <returns>IActionResult - displays issuer authentication form or error page</returns>
        [HttpPost]
        [Route("check3dsEnrollment")]
        public ActionResult Check3dsEnrollment(GatewayApiRequest gatewayApiRequest)
        {
         //   Logger.LogInformation($"PaymentApiController Check3dsEnrollment action SessionId {JsonConvert.SerializeObject(gatewayApiRequest)} gatewayApiRequest.SessionId {gatewayApiRequest.SessionId}");

            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;

            // Retrieve session
            gatewayApiRequest.buildSessionRequestUrl();
            gatewayApiRequest.ApiMethod = GatewayApiClient.GET;

          //  Logger.LogInformation($"gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");

            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

         //   Logger.LogInformation("Get session response -- " + response);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            CheckoutSessionModel checkoutSessionModel = CheckoutSessionModel.toCheckoutSessionModel(response);

         //   Logger.LogInformation($@"checkoutSession.Id {checkoutSessionModel.Id} gatewayApiRequest.SessionId {gatewayApiRequest.SessionId}");

            string secureId = IdUtils.generateSampleId();
            gatewayApiRequest.SecureId = secureId;

            gatewayApiRequest.buildSecureIdRequestUrl();
            gatewayApiRequest.buildPayload();
            gatewayApiRequest.ApiMethod = GatewayApiClient.PUT;


            //add values in session to use it after processing response
            setSessionValue("secureId", secureId);
            setSessionValue("sessionId", checkoutSessionModel.Id);
            setSessionValue("amount", gatewayApiRequest.OrderAmount);
            setSessionValue("currency", gatewayApiRequest.OrderCurrency);
            setSessionValue("orderId", gatewayApiRequest.OrderId);
            setSessionValue("transactionId", gatewayApiRequest.TransactionId);

            response = GatewayApiClient.SendTransaction(gatewayApiRequest);

          //  Logger.LogInformation($"SecureId response {response}");

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //parse response
            SecureIdEnrollmentResponseModel model = null;
            try
            {
                model = SecureIdEnrollmentResponseModel.toSecureIdEnrollmentResponseModel(Request, response);
            }
            catch (Exception e)
            {
              //  Logger.LogError($"Check3dsEnrollment error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }


            //check process result 
          //  Logger.LogInformation($"SecureIdEnrollmentResponseModel {JsonConvert.SerializeObject(model)}");

            if (model.Status != "CARD_ENROLLED")
            {
             //   Logger.LogError($"Check3dsEnrollment was unsuccessful, status {model.Status}");
                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = model.Status,
                    Message = "Card not enrolled in 3DS."
                });
            }

            return View(ViewList["SecureIdPayerAuthenticationForm"], model);
        }

        /// <summary>
        /// This method handles to capture the form['PaRes'] response and send and AUTORIZE call using the information got from the 3DS response.
        /// <summary> 
        /// <param name="PaRes">Needed to retrieve token id to complete API operation</param>
        /// <returns>IActionResult - displays the result or error page</returns>
        [HttpPost]
        [Route("process3ds")]
        public ActionResult Process3dsAuthenticationResult()
        {
            String responseView = ViewList["ApiResponse"];

            //cons
            String AUTHORIZE = "AUTHORIZE";
            String PROCESS_ACS_RESULT = "PROCESS_ACS_RESULT";

            //get secure / session information from session
            String secureId = getSessionValueAsString("secureId");
            String sessionId = getSessionValueAsString("sessionId");
            String amount = getSessionValueAsString("amount");
            String currency = getSessionValueAsString("currency");
            String orderId = getSessionValueAsString("orderId");
            String transactionId = getSessionValueAsString("transactionId");

            //remove values from session
            removeSessionValue("secureId");
            removeSessionValue("sessionId");
            removeSessionValue("amount");
            removeSessionValue("currency");
            removeSessionValue("orderId");
            removeSessionValue("transactionId");

            // Retrieve Payment Authentication Response (PaRes) from request
            String paRes = this.Request.Form["PaRes"];

            //init aux variables
            String response = null;
            GatewayApiRequest gatewayApiRequest = null;


            // Process Access Control Server (ACS) result
            gatewayApiRequest = new GatewayApiRequest();

            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
            gatewayApiRequest.ApiMethod = GatewayApiClient.POST;
            gatewayApiRequest.PaymentAuthResponse = paRes;
            gatewayApiRequest.SecureId = secureId;
            gatewayApiRequest.ApiOperation = PROCESS_ACS_RESULT;
            gatewayApiRequest.buildPayload();
            gatewayApiRequest.buildSecureIdRequestUrl();

            response = GatewayApiClient.SendTransaction(gatewayApiRequest);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //parse response to domain
            SecureIdEnrollmentResponseModel model = null;
            try
            {
                model = SecureIdEnrollmentResponseModel.toSecureIdEnrollmentResponseModel(Request, response);
              //  Logger.LogInformation($"SecureIdEnrollmentResponseModel {JsonConvert.SerializeObject(model)}");

            }
            catch (Exception e)
            {
              //  Logger.LogError($"Process3dsAuthenticationResult error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }


            //create 'authorize' API request in case of SUCCESS response
            if (model.Status != null && "AUTHENTICATION_FAILED" != model.Status)
            {

                //build authorize request
                gatewayApiRequest = new GatewayApiRequest();
                gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
                gatewayApiRequest.ApiMethod = GatewayApiClient.PUT;
                gatewayApiRequest.ApiOperation = AUTHORIZE;

                gatewayApiRequest.SessionId = sessionId;
                gatewayApiRequest.SecureId3D = secureId;
                gatewayApiRequest.OrderCurrency = currency;
                gatewayApiRequest.OrderAmount = amount;
                gatewayApiRequest.TransactionId = transactionId;
                gatewayApiRequest.OrderId = orderId;

                gatewayApiRequest.buildPayload();
                gatewayApiRequest.buildRequestUrl();

                //call api
                response = GatewayApiClient.SendTransaction(gatewayApiRequest);

                //build response view
                this.buildViewData(gatewayApiRequest, response,0);


            }
            else
            {
                //return error view 
                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = model.Status,
                    Message = "3DS Authentication failed."
                });
            }

            return View(responseView);
        }



        /// <summary>
        /// Processes the payment with PayPal.
        /// </summary>
        /// <returns>The payment with pay pal.</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        [HttpPost]
        [Route("processPaymentWithPayPal")]
        public ActionResult ProcessPaymentWithPayPal(GatewayApiRequest gatewayApiRequest)
        {
            //enrich params with paypal information
            gatewayApiRequest.SourceType = "PAYPAL";
            gatewayApiRequest.BrowserPaymentOperation = "PAY";
            gatewayApiRequest.BrowserPaymentPaymentConfirmation = "CONFIRM_AT_PROVIDER";

            //get redirect paypal page
            return browserPayment(gatewayApiRequest);
        }





        /// <summary>
        /// Generic method used for redirec to Browsers payments url.
        /// </summary>
        /// <returns>The payment page.</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        private ActionResult browserPayment(GatewayApiRequest gatewayApiRequest)
        {

            //cons
            string INITIATE_BROWSER_PAYMENT = "INITIATE_BROWSER_PAYMENT";
            string callbackController = "/browserPaymentReceipt";

            //build api request
            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
            gatewayApiRequest.ApiOperation = INITIATE_BROWSER_PAYMENT;
            gatewayApiRequest.ApiMethod = GatewayApiClient.PUT;

            //Build Redirect url

            //Concating the transaction and order ids. It will be used to retrieve the payment result on callback
        //    string returnURL = Microsoft.AspNetCore.Http.Extensions.UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase, callbackController);

            //add query string to return url
         //   returnURL = QueryHelpers.AddQueryString(returnURL, "transactionId", gatewayApiRequest.TransactionId);
          //  returnURL = QueryHelpers.AddQueryString(returnURL, "orderId", gatewayApiRequest.OrderId);

          //  gatewayApiRequest.ReturnUrl = returnURL;

            gatewayApiRequest.buildRequestUrl();
            gatewayApiRequest.buildPayload();

           // Logger.LogInformation($"PaymentApiController action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");


            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //parse response
            InitiateBrowserPaymentResponse initiateResponse = null;
            try
            {
                initiateResponse = InitiateBrowserPaymentResponse.toInitiateBrowserPaymentResponse(response);

            }
            catch (Exception e)
            {
               // Logger.LogError($"BrowserPayment error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }

            //check result
            if ("SUCCESS" != initiateResponse.Result)
            {

              //  Logger.LogInformation($"Browser controller action error response {response}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = initiateResponse.Result,
                    Message = "Browser Payment error."
                });
            }

            //redirect to partner browser payment page 
            return Redirect(initiateResponse.RedirectUrl);
        }



        /// <summary>
        /// Capture the browsers payment callback
        /// </summary>
        /// <returns>The payment receipt.</returns>
        [HttpGet, HttpPost]
        [Route("browserPaymentReceipt")]
        public ActionResult browserPaymentReceipt(String transactionId, String orderId)
        {

            //get order id from page parameter

            //create a gateway parameters request to retrieve the transaction result
            GatewayApiRequest gatewayApiRequest = new GatewayApiRequest();
            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
            gatewayApiRequest.ApiMethod = GatewayApiClient.GET;
            gatewayApiRequest.ApiOperation = "RETRIEVE_TRANSACTION";
            gatewayApiRequest.OrderId = orderId;

            gatewayApiRequest.buildOrderUrl();
            gatewayApiRequest.buildPayload();


            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            //parse response to model
            TransactionResponseModel model = null;
            try
            {
                model = TransactionResponseModel.toTransactionResponseModel(response);
            }
            catch (Exception e)
            {
              //  Logger.LogError($"browserPaymentReceipt error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }

            return View(ViewList["Receipt"], model);
        }



        /// <summary>
        /// Processes the payment with UnionPay.
        /// </summary>
        /// <returns>The payment with UnionPay .</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        [HttpPost]
        [Route("processPaymentWithUnionPay")]
        public ActionResult ProcessPaymentWithUnionPay(GatewayApiRequest gatewayApiRequest)
        {
            //enrich params with Union Pay information
            gatewayApiRequest.SourceType = "UNION_PAY";
            gatewayApiRequest.BrowserPaymentOperation = "AUTHORIZE";

            //get redirect Union Pay page
            return browserPayment(gatewayApiRequest);
        }


        /// <summary>
        /// Processes the Pay through NVP.
        /// </summary>
        /// <returns>The pay through nvp.</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        [HttpPost]
        [Route("processPayThroughNVP")]
        public ActionResult ProcessPayThroughNVP(GatewayApiRequest gatewayApiRequest)
        {
           // Logger.LogInformation($"PaymentApiController Process action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");

            gatewayApiRequest.GatewayApiConfig = GatewayApiConfig;
            gatewayApiRequest.ApiMethod = NVPApiClient.POST;
            gatewayApiRequest.ContentType = NVPApiClient.CONTENT_TYPE;

            string response = NVPApiClient.SendTransaction(gatewayApiRequest);

            buildViewDataNVP(gatewayApiRequest, response);

            return View(ViewList["ApiResponse"]);

        }


        /// <summary>
        /// Processes the masterpass pre requirements before shows the masterbutton.
        /// </summary>
        /// <returns>The masterpass.</returns>
        /// <param name="gatewayApiRequest">Gateway API request.</param>
        [HttpPost]
        [Route("processMasterpass")]
        public ActionResult ProcessMasterpass(GatewayApiRequest gatewayApiRequest)
        {
         //   Logger.LogInformation($"PaymentApiController Process Master Pass action gatewayApiRequest {JsonConvert.SerializeObject(gatewayApiRequest)}");
            String response = null;

            // Create session to use with OPEN_WALLET operation
            GatewayApiRequest gatewaySessionRequest = new GatewayApiRequest(GatewayApiConfig);

            gatewaySessionRequest.buildSessionRequestUrl();
            gatewaySessionRequest.ApiMethod = GatewayApiClient.POST;
            response = GatewayApiClient.SendTransaction(gatewaySessionRequest);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }

            //convert json to model
            CheckoutSessionModel checkoutSessionModel = CheckoutSessionModel.toCheckoutSessionModel(response);

          //  Logger.LogInformation($"Masterpass hostedSession created {response}");

            // Call UPDATE_SESSION to add order information to session
            //update http verb
            GatewayApiRequest gatewayUpdateSessionRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayUpdateSessionRequest.ApiMethod = GatewayApiClient.PUT;

            //update the url appending session id
            gatewayUpdateSessionRequest.buildSessionRequestUrl(checkoutSessionModel.Id);

            gatewayUpdateSessionRequest.OrderId = gatewayApiRequest.OrderId;
            gatewayUpdateSessionRequest.OrderCurrency = gatewayApiRequest.OrderCurrency;
            gatewayUpdateSessionRequest.OrderAmount = gatewayApiRequest.OrderAmount;

            //build payload with order info
            gatewayUpdateSessionRequest.buildPayload();

            response = GatewayApiClient.SendTransaction(gatewayUpdateSessionRequest);

          //  Logger.LogInformation($"Masterpass updated session {response}");

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            // Call OPEN_WALLET to retrieve Masterpass configuration
            //It will use session URL
            GatewayApiRequest gatewayOpenWalletRequest = new GatewayApiRequest(GatewayApiConfig);

            gatewayOpenWalletRequest.buildSessionRequestUrl(checkoutSessionModel.Id);
            gatewayOpenWalletRequest.ApiMethod = GatewayApiClient.POST;

            gatewayOpenWalletRequest.MasterpassOnline = gatewayApiRequest.MasterpassOnline;
            gatewayOpenWalletRequest.MasterpassOriginUrl = gatewayApiRequest.MasterpassOriginUrl;

            gatewayOpenWalletRequest.OrderCurrency = gatewayApiRequest.OrderCurrency;
            gatewayOpenWalletRequest.OrderAmount = gatewayApiRequest.OrderAmount;

            gatewayOpenWalletRequest.buildPayload();

            response = GatewayApiClient.SendTransaction(gatewayOpenWalletRequest);

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            //parse response to model
            MasterpassWalletResponse masterpassWalletResponse = null;
            try
            {
                masterpassWalletResponse = MasterpassWalletResponse.toMasterpassWalletResponse(response);
            }
            catch (Exception e)
            {
               // Logger.LogError($"ProcessMasterpass error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }

           // Logger.LogInformation($"Masterpass wallet configuration {response}");

            // Save this value in HttpSession to retrieve after returning from issuer authentication form
            setSessionValue("sessionId", checkoutSessionModel.Id);
            setSessionValue("amount", gatewayApiRequest.OrderAmount);
            setSessionValue("currency", gatewayApiRequest.OrderCurrency);
            setSessionValue("orderId", gatewayApiRequest.OrderId);

            //set values for view usage
            ViewBag.requestToken = masterpassWalletResponse.RequestToken;
            ViewBag.merchantCheckoutId = masterpassWalletResponse.MerchantCheckoutId;
            ViewBag.allowedCardTypes = masterpassWalletResponse.AllowedCardTypes;

            return View(ViewList["MasterpassButton"]);
        }



        /// <summary>
        /// Process Masterpasses callback response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="oauth_token">Oauth token.</param>
        /// <param name="oauth_verifier">Oauth verifier.</param>
        /// <param name="checkoutId">Checkout identifier.</param>
        /// <param name="checkout_resource_url">Checkout resource URL.</param>
        /// <param name="mpstatus">Mpstatus.</param>
        [HttpGet, HttpPost]
        [Route("masterpassResponse")]
        public ActionResult masterpassResponse(String oauth_token, String oauth_verifier, String checkoutId,
                                                String checkout_resource_url, String mpstatus)
        {

            String UPDATE_SESSION_FROM_WALLET = "UPDATE_SESSION_FROM_WALLET";
            String WALLET_PROVIDER = "MASTERPASS_ONLINE";

            //get session values
            String sessionId = getSessionValueAsString("sessionId");
            String amount = getSessionValueAsString("amount");
            String currency = getSessionValueAsString("currency");
            String orderId = getSessionValueAsString("orderId");

            //remove session values
            removeSessionValue("sessionId");
            removeSessionValue("amount");
            removeSessionValue("currency");
            removeSessionValue("orderId");

            // UPDATE_SESSION_FROM_WALLET - Retrieve payment details from wallet using session ID
            GatewayApiRequest gatewayApiRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayApiRequest.ApiMethod = GatewayApiClient.POST;
            gatewayApiRequest.ApiOperation = UPDATE_SESSION_FROM_WALLET;
            gatewayApiRequest.MasterpassOnline = WALLET_PROVIDER;

            gatewayApiRequest.MasterpassOauthToken = oauth_token;
            gatewayApiRequest.MasterpassOauthVerifier = oauth_verifier;
            gatewayApiRequest.MasterpassCheckoutUrl = checkout_resource_url;

            gatewayApiRequest.buildSessionRequestUrl(sessionId);

            //build json 
            gatewayApiRequest.buildPayload();

            string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

           // Logger.LogInformation($"Masterpass update session : {response}");

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            // Make a payment using the session
            // Construct API request
            GatewayApiRequest gatewayPayApiRequest = new GatewayApiRequest(GatewayApiConfig);
            gatewayPayApiRequest.ApiMethod = GatewayApiClient.PUT;
            gatewayPayApiRequest.ApiOperation = "PAY";
            gatewayPayApiRequest.SessionId = sessionId;

            //order info
            gatewayPayApiRequest.OrderAmount = amount;
            gatewayPayApiRequest.OrderId = orderId;
            gatewayPayApiRequest.OrderCurrency = currency;
            gatewayPayApiRequest.TransactionId = IdUtils.generateSampleId();

            //build payload
            gatewayPayApiRequest.buildPayload();

            gatewayPayApiRequest.buildRequestUrl();

            response = GatewayApiClient.SendTransaction(gatewayPayApiRequest);

           // Logger.LogInformation($"Masterpass PAY operation : {response}");

            //validate transaction response
            if (JsonHelper.isErrorMessage(response))
            {
                return View("Error", ErrorViewModel.toErrorViewModel(getRequestId(), response));
            }


            //parse response to default transaction response model
            TransactionResponseModel model = null;
            try
            {
                model = TransactionResponseModel.fromMasterpassResponseToTransactionResponseModel(response);
            }
            catch (Exception e)
            {
              //  Logger.LogError($"MasterpassResponse error : {JsonConvert.SerializeObject(e)}");

                return View("Error", new ErrorViewModel
                {
                    RequestId = getRequestId(),
                    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    Message = e.Message
                });
            }

            return View(ViewList["Receipt"], model);
        }


    }
}