//     PushGateway.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Handles interaction between an application provider and the push gateway.
    /// </summary>
    public sealed class PushGateway
    {
        static SettingsSection _settings;

        static PushGateway()
        {
            _settings = ConfigurationManager.GetSection("pushGateway") as SettingsSection;
        }

        string _baseUrl, _appId, _apiKey;

        /// <summary>
        /// Connect to a push gateway using default configuration values.
        /// </summary>
        public PushGateway()
        {
            if (_settings == null)
                throw new ConfigurationErrorsException("Push gateway is not configured.");
            _baseUrl = _settings.BaseUrl;
            _appId = _settings.AppId;
            _apiKey = _settings.ApiKey;
            if (!_baseUrl.EndsWith("/"))
                _baseUrl += '/';
        }

        /// <summary>
        /// Register a device with the push gateway. This must be called before a message can be
        /// delivered to a given device.
        /// </summary>
        /// <param name="platform">The device platform.</param>
        /// <param name="deviceToken">The device token or registration ID supplied by the device.</param>
        /// <param name="segments">The segments or delivery groups to which a device belongs.</param>
        public void Register(Platform platform, string deviceToken, params string[] segments)
        {
            var content = new JObject(
                new JProperty("platform", platform.ToPlatformName()),
                new JProperty("token", deviceToken),
                new JProperty("segments", segments)
                );
            this.Send("register", content.ToString());
        }

        /// <summary>
        /// Register a device with the push gateway. This must be called before a message can be
        /// delivered to a given device.
        /// </summary>
        /// <param name="platform">The device platform.</param>
        /// <param name="deviceToken">The device token or registration ID supplied by the device.</param>
        /// <param name="segments">The segments or delivery groups to which a device belongs.</param>
        /// <returns>Asynchronous task.</returns>
        public Task RegisterAsync(Platform platform, string deviceToken, params string[] segments)
        {
            return Task.Factory.StartNew(() => this.Register(platform, deviceToken, segments));
        }

        /// <summary>
        /// Unregisters a device with the push gateway. The device will no longer receive push notifications
        /// for this app.
        /// </summary>
        /// <param name="platform">The device platform.</param>
        /// <param name="deviceToken">The previously registered device token or registration ID.</param>
        public void Unregister(Platform platform, string deviceToken)
        {
            var content = new JObject(
                new JProperty("platform", platform.ToPlatformName()),
                new JProperty("token", deviceToken)
                );
            this.Send("unregister", content.ToString());
        }

        /// <summary>
        /// Unregisters a device with the push gateway. The device will no longer receive push notifications
        /// for this app.
        /// </summary>
        /// <param name="platform">The device platform.</param>
        /// <param name="deviceToken">The previously registered device token or registration ID.</param>
        /// <returns>Asynchronous task.</returns>
        public Task UnregisterAsync(Platform platform, string deviceToken)
        {
            return Task.Factory.StartNew(() => this.Unregister(platform, deviceToken));
        }

        /// <summary>
        /// Send a push notification to one or more devices.
        /// </summary>
        /// <typeparam name="T">The notification payload type for the given platform.</typeparam>
        /// <param name="notification">The notification data.</param>
        /// <returns>The results of the send operation.</returns>
        public SendResult Send<T>(PushNotification<T> notification) where T : INotificationPayload, new()
        {
            var response = (HttpWebResponse)this.Send("send", JsonConvert.SerializeObject(notification));
            using (var stream = response.GetResponseStream())
            {
                return JsonConvert.DeserializeObject<SendResult>(new StreamReader(stream).ReadToEnd());
            }
        }

        /// <summary>
        /// Send a push notification to one or more devices.
        /// </summary>
        /// <typeparam name="T">The notification payload type for the given platform.</typeparam>
        /// <param name="notification">The notification data.</param>
        /// <returns>Asynchronous task.</returns>
        public Task<SendResult> SendAsync<T>(PushNotification<T> notification) where T : INotificationPayload, new()
        {
            return Task<SendResult>.Factory.StartNew(() => this.Send(notification));
        }

        private HttpWebResponse Send(string path, string content)
        {
            var body = Encoding.UTF8.GetBytes(content);

            var request = (HttpWebRequest)WebRequest.Create(_baseUrl + path);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", string.Format("APIKEY appId={0},apiKey={1}", _appId, _apiKey));
            using (var stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            }

            try
            {
                return (HttpWebResponse) request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    try
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        {
                            System.Diagnostics.Trace.WriteLine("Server response:\n" +
                                                               new StreamReader(stream).ReadToEnd());
                        }
                    }
                    catch
                    {
                    }
                }
                throw;
            }
        }
    }
}
