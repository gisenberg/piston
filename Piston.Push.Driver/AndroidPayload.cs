//     AndroidPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds data specific to push notifications sent to Android devices.
    /// </summary>
    public class AndroidPayload : INotificationPayload
    {
        public AndroidPayload()
        {
            this.Data = new Dictionary<string, object>();
        }

        /// <summary>
        /// An identifier used to group a series of messages. When multiple messages with the same
        /// collapse key are queued for delivery to a device, only the most recent message is actually
        /// delivered. The default is no collapse key.
        /// </summary>
        [JsonProperty("collapse_key")]
        public string CollapseKey { get; set; }

        /// <summary>
        /// When set to true, indicates that the message should not be sent immediately if the device
        /// is idle. The message will be delivered only when the device becomes active. The default
        /// value is false.
        /// </summary>
        [JsonProperty("delay_while_idle")]
        public bool? DelayWhileIdle { get; set; }

        /// <summary>
        /// When set to the property name of your application, messages will only be sent to device
        /// tokens that match the package name. The default is no restriction.
        /// </summary>
        [JsonProperty("restricted_package_name")]
        public bool? RestrictedPackageName { get; set; }

        /// <summary>
        /// When set to true, the message is accepted but not actually delivered to the device. This
        /// can be used to test the push gateway without actually delivering a notification. The default
        /// value is false.
        /// </summary>
        [JsonProperty("dry_run")]
        public bool? DryRun { get; set; }

        /// <summary>
        /// A dictionary containing zero or more fields of raw data to deliver along with the
        /// notification. This data will be sent to the application as 'extras'.
        /// </summary>
        [JsonProperty("data")]
        public IDictionary<string, object> Data { get; private set; }

        string INotificationPayload.GetPlatformName()
        {
            return "gcm";
        }
    }
}
