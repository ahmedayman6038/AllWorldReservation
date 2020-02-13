using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using AllWorldReservation.BL.Utils;
using System.Web;

namespace AllWorldReservation.web.Models
{
    public class SecureIdEnrollmentResponseModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string ResponseUrl { get; set; }
        public string AcsUrl { get; set; }
        public string PaReq { get; set; }
        public string MdValue { get; set; }

        public static SecureIdEnrollmentResponseModel toSecureIdEnrollmentResponseModel(HttpRequestBase Request, string response)
        {
            JObject jObject = JObject.Parse(response);

            SecureIdEnrollmentResponseModel model = new SecureIdEnrollmentResponseModel();
            model.Status = jObject["3DSecure"]["summaryStatus"].Value<string>();
            model.AcsUrl = jObject["3DSecure"]["authenticationRedirect"]["customized"]["acsUrl"].Value<string>();
            model.PaReq = jObject["3DSecure"]["authenticationRedirect"]["customized"]["paReq"].Value<string>();
            model.MdValue = IdUtils.generateSampleId();

           // model.ResponseUrl = UrlHelper.GenerateUrl(Request.Url.Scheme, Request.Url.Host, Request.Url.LocalPath, "/process3ds");

            return model;
        }
    }
}
