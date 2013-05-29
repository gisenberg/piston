//     RabbitContext.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using RabbitMQ.Client;
using System;
using System.Configuration;
using System.Diagnostics;

namespace Piston.Push.Core
{
    public abstract class RabbitContext : IDisposable
    {
        const int BrokerRetryWaitSeconds = 10;

        private static readonly SettingsSection _settings = ((SettingsSection)ConfigurationManager.GetSection("pushGateway"));
        private static readonly ConnectionFactory _factory = new ConnectionFactory();
        private static IConnection _conn;
        private IModel _model;

        static RabbitContext()
        {
            _factory.Uri = _settings.RabbitUri;
        }

        public static IConnection Connection
        {
            get
            {
                if (_conn == null || !_conn.IsOpen)
                {
                    lock (_factory)
                    {
                        if (_conn == null || !_conn.IsOpen)
                        {
                            while (true)
                            {
                                try
                                {
                                    _conn = _factory.CreateConnection();
                                    break;
                                }
                                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                                {
                                    Trace.TraceError("Unable to connect to broker: " + ex.Message);
                                    System.Threading.Thread.Sleep(BrokerRetryWaitSeconds * 1000);
                                }
                            }
                        }
                    }
                }
                return _conn;
            }
        }

        public IModel Model
        {
            get
            {
                if (_model == null || !_model.IsOpen)
                {
                    _model = Connection.CreateModel();
                    Connection.AutoClose = true;
                }
                return _model;
            }
        }

        public virtual void Dispose()
        {
            this.Model.Dispose();
        }
    }
}
