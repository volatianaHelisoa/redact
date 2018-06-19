using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RedactApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{hash}",
                defaults: new { controller = "Login", action = "Accueil", hash = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LoginPass",
                url: "{controller}/{action}/{token}",
                defaults: new { controller = "Home", action = "Index", token = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Home",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "HomeAction",
                url: "{controller}/{action}/{currentHash}",
                defaults: new { controller = "Home", action = "Index", currentHash = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HomeChange",
                url: "{controller}/ListeUser/{action}/{data}",
                defaults: new { controller = "Home", action = "ListeUser", data = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ContactChange",
                url: "{controller}/ListeContact/{action}/{data}",
                defaults: new { controller = "Contact", action = "ListeContact", data = UrlParameter.Optional }
            );

           

            routes.MapRoute(
                name: "SelectItemTheme",
                url: "{controller}/LoadRedacteurByTheme/{action}/{theme}",
                defaults: new { controller = "Commandes", action = "LoadRedacteurByTheme", theme = UrlParameter.Optional }
            );
            routes.MapRoute(
                "404-PageNotFound",
                "{*url}",
                new { controller = "Error", action = "Index" }
            );
        }
    }
}
