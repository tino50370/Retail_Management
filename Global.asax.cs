using RetailKing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Http;
using RetailKing.App_Start;
using RetailKing.Models;
using RetailKing.Controllers;
using System.Timers;
using System.Threading.Tasks;
using System.Net.Http.Headers;


namespace RPA
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });
         
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            // config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.EnableCors();

            // Web API routes

            config.Routes.MapHttpRoute(
               name: "SearchApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );


        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Register(GlobalConfiguration.Configuration);
            RegisterRoutes(RouteTable.Routes);
            RegisterGlobalFilters(GlobalFilters.Filters);
           

           /* Timer aTimer = new Timer();
            aTimer.Interval = 600 * 1000;
            aTimer.Elapsed +=  (sender, e) =>  aTimer_Tick();
            aTimer.Start();

            MailSettingsController mset = new MailSettingsController();
            mset.StartAllServices(); */
            
        }
        private void aTimer_Tick()
        {
            MailSettingsController mset = new MailSettingsController();
            mset.StartAllServices();
            
        }
    }
}