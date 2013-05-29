//     DependencyConfig.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Piston.Push.Core;

namespace Piston.Push.Web
{
    public class DependencyConfig
    {
        public static void RegisterTypes(HttpConfiguration config)
        {
            var container = new UnityContainer();
            container.RegisterType<ITokenStore, MongoTokenStore>();
            container.RegisterType<INotificationPublisher, RabbitPublisher>();
            
            DependencyResolver.SetResolver(new Unity.Mvc3.UnityDependencyResolver(container));
            config.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}