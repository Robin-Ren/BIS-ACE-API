using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BisAceAPI;

namespace BisAceAPI
{
#pragma warning disable CS1591
    public class WebApiApplication : System.Web.HttpApplication
#pragma warning restore CS1591
    {
#pragma warning disable CS1591
        protected void Application_Start()
#pragma warning restore CS1591
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
