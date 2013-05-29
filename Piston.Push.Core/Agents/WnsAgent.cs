//     WnsAgent.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Piston.Push.Core
{
    public class WnsAgent : Agent
    {
        static WnsElement _settings = Settings.Wns;

        string _authToken;

        public WnsAgent(AppSecret secret, INotificationConsumer consumer, ITokenStore store)
            : base(secret, consumer, store, _settings)
        {
        }

        protected override void OnNotify(NotificationEventArgs e)
        {
            if (_authToken == null)
                this.Authenticate();

            var payload = e.Payload.ToObject<WnsPayload>();
            var req = (HttpWebRequest)WebRequest.Create(e.DeviceToken);
            req.Method = "POST";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(payload.Body);
            req.ContentLength = bodyBytes.Length;
            payload.Headers["Authorization"] = "Bearer " + _authToken;

            foreach (var header in payload.Headers)
                req.Headers.Add(header.Key, header.Value);

            var uri = new Uri(e.DeviceToken);

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

            Trace.TraceInformation("[{0}] Response={1}",
                this.AgentId, (int)response.StatusCode);

            switch ((int)response.StatusCode)
            {
                case 200:
                    break;
                case 400:
                case 405:
                    throw new UndeliverableException("Malformed request.");
                case 401:
                    _authToken = null;
                    throw new TemporaryUndeliverableException("Access token invalid or expired.");
                case 403:
                    throw new UndeliverableException("Unauthorized device token");
                case 404:
                case 410:
                    this.Store.Unregister(this.Consumer.AppId, Platform.Wns, e.DeviceToken, null);
                    throw new UndeliverableException("Invalid device token.");
                case 406:
                    throw new TemporaryServiceException("Throttling limit reached.");
                case 413:
                    throw new UndeliverableException("Notification too large.");
                default:
                    throw new UndeliverableException("Unrecognized response code " + (int)response.StatusCode);
            }
        }

        private void Authenticate()
        {
            Trace.TraceInformation("[{0}] Authenticating with {1}, AppId = {2}",
                this.AgentId, _settings.AuthenticationUrl, this.Secret.AppId);

            var req = (HttpWebRequest)WebRequest.Create(_settings.AuthenticationUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var body = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com",
                Uri.EscapeDataString(this.Secret.AppId),
                Uri.EscapeDataString(this.Secret.Key));
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            req.ContentLength = bodyBytes.Length;
            using (var stream = req.GetRequestStream())
            {
                stream.Write(bodyBytes, 0, bodyBytes.Length);
            }
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                Trace.TraceError(string.Format("[{0}] Authentication failed: {1}",
                    this.AgentId, ex.Message));

                if (ex.Response != null)
                    res = (HttpWebResponse)ex.Response;
                else
                    throw new TemporaryServiceException("Authentication failed: " + ex.Message);
            }
            if (res.StatusCode != HttpStatusCode.OK)
                throw new TemporaryServiceException("Authentication failed: " + res.StatusCode);

            try
            {
                using (var stream = res.GetResponseStream())
                {
                    using (var sr = new System.IO.StreamReader(stream))
                    {
                        var token = JsonConvert.DeserializeObject<OAuthToken>(sr.ReadToEnd());
                        _authToken = token.access_token;
                        Trace.TraceInformation(string.Format("[{0}] Received authentication token {1}",
                            this.AgentId, _authToken));
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(string.Format("[{0}] Bad authentication response: {1}",
                    this.AgentId, ex.Message));
                throw new TemporaryServiceException("Bad authentication response: " + ex.Message);
            }
        }

        class OAuthToken
        {
            public string access_token = null;
            public string token_type = null;
        }
    }
}
