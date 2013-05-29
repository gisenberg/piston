//     RabbitPublisher.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace Piston.Push.Core
{
    public class RabbitPublisher : RabbitContext, INotificationPublisher
    {
        const byte DeliveryMode = 2; // persistent messages
        const string Exchange = ""; // default exchange; direct routing

        public void Enqueue(string appId, Platform platform, string token, uint expiry, object payload)
        {
            var key = string.Format("{0}.{1}", appId, platform.ToString().ToLower());
            var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var props = this.Model.CreateBasicProperties();

            props.ContentType = "application/json";
            props.DeliveryMode = DeliveryMode;
            props.Headers = new Dictionary<string,string> { { "device-token", token }, { "expiry", expiry.ToString() } };
            this.Model.BasicPublish(Exchange, key, props, body);
        }
    }
}
