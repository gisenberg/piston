//     WebApiConfig.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Web.Http;

namespace Piston.Push.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "Push.Register",
                routeTemplate: "register",
                defaults: new { controller = "Push", action = "Register" }
                );
            config.Routes.MapHttpRoute(
                name: "Push.Unregister",
                routeTemplate: "unregister",
                defaults: new { controller = "Push", action = "Unregister" }
                );
            config.Routes.MapHttpRoute(
                name: "Push.Push",
                routeTemplate: "send",
                defaults: new { controller = "Push", action = "Push" }
                );

            config.MessageHandlers.Add(new AuthorizationHandler());

            config.Filters.Add(new RestHandleErrorAttribute());
        }
    }
}
