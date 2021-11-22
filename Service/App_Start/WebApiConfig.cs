using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
using Service.App_Start;

namespace Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            //https://code.msdn.microsoft.com/ASPNET-WebAPI-Enable-Cors-666b88eb
            //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.0
            //https://stackoverflow.com/questions/27504256/mvc-web-api-no-access-control-allow-origin-header-is-present-on-the-requested

            var ipsincludes = ConfigurationManager.AppSettings.Get("ipsincludes");
            var cors = new EnableCorsAttribute(ipsincludes, "*", "*");
            
            config.EnableCors(cors);
            // Rutas de API web

            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new CustomLogHandler());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
