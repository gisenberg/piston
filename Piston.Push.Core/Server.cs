//     Server.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Piston.Push.Core
{
    public class Server : IDisposable
    {
        static readonly Type TokenStoreType = typeof(MongoTokenStore);
        static readonly Type NotificationConsumerType = typeof(RabbitConsumer);

        List<Agent> _agents = new List<Agent>();
        IUnityContainer _container;

        public void Start()
        {
            _container = new UnityContainer();
            _container.RegisterType(typeof(ITokenStore), TokenStoreType);

            var metaStore = new MongoMetaStore();
            var appList = metaStore.GetApps();
            int appCount = 0;
            foreach (var app in metaStore.GetApps())
            {
                appCount++;
                foreach (var p in app.Platforms)
                {
                    Platform platform;
                    if (Enum.TryParse<Platform>(p.Key, true, out platform))
                    {
                        using (IUnityContainer subContainer = _container.CreateChildContainer())
                        {
                            Trace.TraceInformation("Spinning up agent: AppId={0}, Platform={1}", app.AppId, platform.ToString());
                            subContainer.RegisterType(typeof(INotificationConsumer), NotificationConsumerType,
                                new InjectionConstructor(platform, app.AppId));
                            this.StartAgent(subContainer, platform, p.Value);
                        }
                    }
                }
            }
            System.Net.ServicePointManager.DefaultConnectionLimit = Math.Max(2, appCount*2);
        }

        public void Dispose()
        {
            foreach (var agent in _agents)
            {
                agent.Dispose();
            }
        }

        private void StartAgent(IUnityContainer container, Platform platform, AppSecret secret)
        {
            Agent agent;

            var secretParam = new ParameterOverride("secret", secret);

            switch (platform)
            {
                case Platform.Apns:
                    agent = container.Resolve<ApnsAgent>(secretParam);
                    break;
                case Platform.Mpns:
                    agent = container.Resolve<MpnsAgent>(secretParam);
                    break;
                case Platform.Gcm:
                    agent = container.Resolve<GcmAgent>(secretParam);
                    break;
                case Platform.Wns:
                    agent = container.Resolve<WnsAgent>(secretParam);
                    break;
                default:
                    return;
            }

            _agents.Add(agent);
            agent.Start();
        }
    }
}
