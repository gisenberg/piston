//     IosPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds data specific to push notifications sent to iOS devices.
    /// </summary>
    [JsonConverter(typeof(IosPayload.IosPayloadConverter))]
    public class IosPayload : INotificationPayload
    {
        JObject _aps;

        public IosPayload()
        {
            this.Data = new Dictionary<string, object>();
            _aps = new JObject();
        }

        /// <summary>
        /// A dictionary containing zero or more fields of raw data to deliver along with the
        /// notification.
        /// </summary>
        public IDictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Update the badge as part of this notification. This updates the small red number on the application icon.
        /// A value of zero will remove any existing badge number.
        /// </summary>
        /// <param name="count">The number that should be displayed on the application icon, or zero to remove the number.</param>
        public void SetBadge(int count)
        {
            _aps["badge"] = count;
        }

        /// <summary>
        /// Speficy which sound to play with the notification. By default, no sound is played.
        /// </summary>
        /// <param name="sound">The name of the sound in the application bundle, or 'default' for the default alert sound.</param>
        public void SetSound(string sound)
        {
            _aps["sound"] = sound;
        }

        /// <summary>
        /// Specify that the notification should display a textual alert to the user. The alert is displayed in a dialog
        /// with 'Close' and 'View' buttons. If the user presses 'View' the application will be launched.
        /// </summary>
        /// <param name="body">The alert text to display to the user.</param>
        public void SetAlert(string body)
        {
            _aps["alert"] = body;
        }

        /// <summary>
        /// Specify that the notification should display a textual alert to the user. This method allows for the alert
        /// text to be localized.
        /// </summary>
        /// <param name="body">The text of the alert message.</param>
        /// <param name="launchImage">The filename of an image in the app bundle, with or without extension. This overrides
        /// an existing launch image (typically an app snapshot). The default value is null.</param>
        /// <param name="actionLocKey">Specifies a localization key for the right 'View' button. If set to null, the alert
        /// has only a single 'OK' button that dismisses the alert.</param>
        /// <param name="locKey">A key to an alert message in the app's localization file. See Apple documentation for the
        /// string format.</param>
        /// <param name="locArgs">Specify arguments to be applied to the string format specified by locKey.</param>
        public void SetAlert(string body, string launchImage = null, string actionLocKey = null, string locKey = null, IEnumerable<string> locArgs = null)
        {
            JObject alert;
            alert = new JObject(new JProperty("body", body));
            if (launchImage != null)
                alert["launch-image"] = launchImage;
            if (actionLocKey != null)
                alert["action-loc-key"] = actionLocKey;
            if (locKey != null)
            {
                alert["loc-key"] = locKey;
                alert["loc-args"] = new JArray(locArgs);
            }
            _aps["alert"] = alert;
        }

        string INotificationPayload.GetPlatformName()
        {
            return "apns";
        }

        public class IosPayloadConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(IosPayload);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var payload = (IosPayload)value;
                var output = JObject.FromObject(payload.Data);
                output["aps"] = payload._aps;
                output.WriteTo(writer);
            }
        }
    }
}
