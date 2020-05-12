using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AllWorldReservation.BL.Mapper;
using System.Web.Security;

namespace AllWorldReservation.web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfiguration.Configure();
        }

        protected void Application_BeginRequest()
        {
            if (FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            }
        }

        public class AutoMapperConfiguration
        {
            public static void Configure()
            {
                Mapper.Initialize(x =>
                {
                    x.AddProfile<DomainProfile>();
                });

                Mapper.Configuration.AssertConfigurationIsValid();
            }
        }
    }
}
