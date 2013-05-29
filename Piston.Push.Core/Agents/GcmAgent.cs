//     GcmAgent.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Piston.Push.Core
{
    public class GcmAgent : Agent
    {
        const int MaxTimeToLive = 2419200;
        static GcmElement _settings = Settings.Gcm;

        public GcmAgent(AppSecret secret, INotificationConsumer consumer, ITokenStore store)
            : base(secret, consumer, store, _settings)
        {
        }

        protected override void OnNotify(NotificationEventArgs e)
        {
            var payload = e.Payload.ToObject<GcmPayload>();
            payload.RegistrationIds = new string[] { e.DeviceToken };
            if (e.Expiry > 0)
                payload.TimeToLive = Math.Min(MaxTimeToLive, Math.Max(0, e.Expiry - DateTime.Now.ToUnixEpoch()));
            var payloadJson = JsonConvert.SerializeObject(payload, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            var req = (HttpWebRequest)WebRequest.Create(_settings.SendUrl);
            req.Method = "POST";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(payloadJson);
            req.ContentLength = bodyBytes.Length;
            req.ContentType = "application/json";
            req.Headers.Add("Authorization", "key=" + this.Secret.Key);

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

            int status = (int)response.StatusCode;
            string responseStr;
            using (var stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    responseStr = sr.ReadToEnd();
                }
            }
            
            GcmResponse responseData;
            try
            {
                responseData = JsonConvert.DeserializeObject<GcmResponse>(responseStr);
            }
            catch (Exception ex)
            {
                // With a status code of 200, always assume success to prevent spam.
                Trace.TraceWarning("[{0}] Response={1}, Bad response body: {2}", this.AgentId, response.StatusCode, ex.Message);
                return;
            }

            if (status == 400 || status == 404)
                throw new UndeliverableException("Bad request.");
            else if (status == 401)
                throw new FatalServiceException("Bad authorization.");
            else if (status >= 500 && status < 600)
                throw new TemporaryServiceException("Temporary service failure.");
            else if (status != 200)
                throw new UndeliverableException("Unrecognized status code.");

            Trace.TraceInformation("[{0}] Response={1}, MulticastId={2}, Success={3}, Failure={4}, CanonicalIds={5}",
                this.AgentId, response.StatusCode, responseData.MulticastId, responseData.Success, responseData.Failure, responseData.CanonicalIds);

            bool isMulticast = payload.RegistrationIds.Length > 1;

            if ((responseData.Failure > 0 || responseData.CanonicalIds > 0) && responseData.Results != null)
            {
                for (int i = 0; i < responseData.Results.Length; i++)
                {
                    var mr = responseData.Results[i];
                    if (mr.MessageId != null)
                    {
                        if (!string.IsNullOrEmpty(mr.RegistrationId))
                        {
                            this.Store.UpdateRegistrationId(this.Consumer.AppId, Platform.Gcm, payload.RegistrationIds[i], mr.RegistrationId);
                        }
                    }
                    else
                    {
                        if (string.Compare("Unavailable", mr.Error, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // For multicast messages, we don't support redelivery
                            if (!isMulticast)
                                throw new TemporaryUndeliverableException("Device is unavailable.");
                        }
                        else if (string.Compare("NotRegistered", mr.Error, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Store.Unregister(this.Consumer.AppId, Platform.Gcm, payload.RegistrationIds[i], null);
                            if(!isMulticast)
                                throw new UndeliverableException("Device token not registered.");
                        }
                        else if (string.Compare("InvalidRegistration", mr.Error, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this.Store.Unregister(this.Consumer.AppId, Platform.Gcm, payload.RegistrationIds[i], null);
                            if (!isMulticast)
                                throw new UndeliverableException("Invalid device registration.");
                        }
                        else if (!isMulticast)
                        {
                            throw new UndeliverableException("Other send error: " + mr.Error);
                        }
                    }
                }
            }
        }

        private class GcmMessageResult
        {
            [JsonProperty("message_id")]
            public string MessageId { get; set; }

            [JsonProperty("registration_id")]
            public string RegistrationId { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }

        private class GcmResponse
        {
            [JsonProperty("multicast_id")]
            public string MulticastId { get; set; }

            [JsonProperty("success")]
            public int Success { get; set; }

            [JsonProperty("failure")]
            public int Failure { get; set; }

            [JsonProperty("canonical_ids")]
            public int CanonicalIds { get; set; }

            [JsonProperty("results")]
            public GcmMessageResult[] Results { get; set; }
        }
    }
}
