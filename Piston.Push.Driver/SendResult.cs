//     SendResult.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds the results of a notification send request.
    /// </summary>
    public class SendResult
    {
        /// <summary>
        /// Indicates whether or not the request was successful. Note that if a notification
        /// is sent to multiple devices, the request is considered successful if any messages
        /// could be queued. A failure typically means the device tokens or payload were invalid.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Indicates how many individual messages (one message per device) were enqueued. This is
        /// the total of devices belonging to the requested segments, plus any explicitly specified
        /// device tokens, minus device tokens that are no longer registered.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets a list of device tokens that are not registered with the push gateway. The application
        /// should stop sending notifications to these devices.
        /// </summary>
        [JsonProperty("badTokens")]
        public string[] BadTokens { get; set; }
    }
}
