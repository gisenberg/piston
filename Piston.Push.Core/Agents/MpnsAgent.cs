//     MpnsAgent.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Piston.Push.Core
{
    public class MpnsAgent : Agent
    {
        static MpnsElement _settings = Settings.Mpns;

        public MpnsAgent(AppSecret secret, INotificationConsumer consumer, ITokenStore store)
            : base(secret, consumer, store, _settings)
        {
        }

        protected override void OnNotify(NotificationEventArgs e)
        {
            var payload = e.Payload.ToObject<MpnsPayload>();
            var req = (HttpWebRequest)WebRequest.Create(e.DeviceToken);
            req.Method = "POST";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(payload.Body);
            req.ContentLength = bodyBytes.Length;
            req.ContentType = "text/xml";
            foreach (var header in payload.Headers)
                req.Headers.Add(header.Key, header.Value);
            var uri = new Uri(e.DeviceToken);
            if (uri.Scheme == "https")
            {
                req.ClientCertificates.Add(this.Secret.GetX509Certificate());
            }

            using (var stream = req.GetRequestStream())
            {
                stream.Write(bodyBytes, 0, bodyBytes.Length);
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    response = (HttpWebResponse)ex.Response;
                else
                    throw new TemporaryUndeliverableException("No response returned.");
            }

            string noteStatus = response.Headers["X-NotificationStatus"];
            string subStatus = response.Headers["X-SubscriptionStatus"];
            string connStatus = response.Headers["X-DeviceConnectionStatus"];
            if(noteStatus == null || subStatus == null || connStatus == null)
                throw new UndeliverableException("Missing response headers.");

            Trace.TraceInformation("[{0}] Response={1}, NotificationStatus={2}, DeviceConnectionStatus={3}, SubscriptionStatus={4}",
                this.AgentId, response.StatusCode, noteStatus, connStatus, subStatus);

            switch ((int)response.StatusCode)
            {
                case 200:
                    if (string.Compare("QueueFull", noteStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        throw new TemporaryServiceException("Remote queue is full.");
                    else if (string.Compare("Suppressed", noteStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        throw new UndeliverableException("Messager was suppressed.");
                    break;
                case 400:
                    throw new UndeliverableException("Malformed request.");
                case 401:
                    throw new UndeliverableException("Bad authorization or device token.");
                case 404:
                    this.Store.Unregister(this.Consumer.AppId, Platform.Mpns, e.DeviceToken, null);
                    throw new UndeliverableException("Invalid subscription.");
                case 406:
                    throw new TemporaryUndeliverableException("Daily throttling limit reached.");
                case 412:
                    throw new TemporaryUndeliverableException("Device inactive.");
                case 503:
                    throw new TemporaryServiceException("General service error.");
                default:
                    throw new UndeliverableException("Unrecognized response code.");
            }
        }
    }
}
