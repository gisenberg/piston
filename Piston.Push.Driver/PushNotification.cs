//     PushNotification.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds platform-independent data for a push notification, as well as
    /// the platform-specific payload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PushNotification<T> where T : INotificationPayload, new()
    {
        public PushNotification()
        {
            this.Payload = new T();
            this.Platform = this.Payload.GetPlatformName();
            this.Tokens = new List<string>();
            this.Segments = new List<string>();
        }

        public PushNotification(T payload) : this()
        {
            this.Payload = payload;
        }

        /// <summary>
        /// Specifies the platform to which the notification will be delivered.
        /// </summary>
        [JsonProperty("platform")]
        internal string Platform { get; private set; }

        /// <summary>
        /// Specifies zero or more tokens identifying the device and application to
        /// which the notification will be delivered.
        /// </summary>
        [JsonProperty("tokens")]
        public IList<string> Tokens { get; private set; }

        /// <summary>
        /// Specifies zero or more segments (groups of devices) to which the
        /// notification will be delivered.
        /// </summary>
        [JsonProperty("segments")]
        public IList<string> Segments { get; private set; }

        /// <summary>
        /// Specifies the time that the notification will expire in UNIX epoch format
        /// (seconds since 1970-1-1). If the notification has not been delivered by this
        /// time then it may be dropped.
        /// </summary>
        [JsonProperty("expiry")]
        public uint Expiry { get; set; }

        /// <summary>
        /// The platform-dependent payload data.
        /// </summary>
        [JsonProperty("payload")]
        public T Payload { get; set; }
    }
}
