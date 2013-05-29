//     NotificationEventArgs.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Piston.Push.Core
{
    public class NotificationEventArgs : EventArgs, IDisposable
    {
        public string DeviceToken { get; private set; }
        public uint Expiry { get; private set; }
        public JObject Payload { get; private set; }
        public object Tag { get; set; }

        readonly INotificationConsumer _consumer;
        bool _acked;

        public NotificationEventArgs(INotificationConsumer consumer, string deviceToken, uint expiry, JObject payload, object tag)
        {
            _consumer = consumer;
            _acked = false;
            this.DeviceToken = deviceToken;
            this.Expiry = expiry;
            this.Payload = payload;
            this.Tag = tag;
        }

        public void Accept()
        {
            if (_acked) return;
            _consumer.Accept(this);
            _acked = true;
        }

        public void Reject(bool requeue)
        {
            if (_acked) return;
            _consumer.Reject(this, requeue);
            _acked = true;
        }

        public void Dispose()
        {
            this.Accept();
        }

        public override string ToString()
        {
            return string.Format("DeviceToken={0}, DeliveryTag={1}, Payload={2}", this.DeviceToken, this.Tag.ToString(), JsonConvert.SerializeObject(this.Payload));
        }
    }
}
