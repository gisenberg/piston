//     WindowsPhonePayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds data specific to push notifications sent to Android devices.
    /// </summary>
    public sealed class WindowsPhonePayload : INotificationPayload
    {
        const string OuterFormat = "<?xml version=\"1.0\" encoding=\"utf-8\"?><wp:Notification xmlns:wp=\"WPNotification\">{0}</wp:Notification>";

        public WindowsPhonePayload()
        {
            this.Headers = new Dictionary<string, string>();
            this.SetRaw("");
        }

        WindowsPhoneNotificationClass _notificationClass;

        [JsonProperty("headers")]
        internal Dictionary<string, string> Headers { get; private set; }

        [JsonProperty("body")]
        internal string Body { get; set; }

        [JsonIgnore]
        internal string Target
        {
            get
            {
                return this.Headers["X-WindowsPhone-Target"];
            }
            set
            {
                if (value == null && this.Headers.ContainsKey("X-WindowsPhone-Target"))
                    this.Headers.Remove("X-WindowsPhone-Target");
                else
                    this.Headers["X-WindowsPhone-Target"] = value;
                this.ApplyNotificationClass();
            }
        }

        /// <summary>
        /// Gets or sets the notification class, controlling how the message will be batched
        /// for delivery to the device. By default, the message will be delivered immediately.
        /// See the WindowsPhoneNotificationClass enum for more information.
        /// </summary>
        [JsonIgnore]
        public WindowsPhoneNotificationClass NotificationClass
        {
            get
            {
                return _notificationClass;
            }
            set
            {
                _notificationClass = value;
                this.ApplyNotificationClass();
            }
        }
        
        private void ApplyNotificationClass()
        {
            int val = 3;
            if (this.Headers.ContainsKey("X-WindowsPhone-Target"))
            {
                switch (this.Headers["X-WindowsPhone-Target"])
                {
                    case "token":
                        val = 1;
                        break;
                    case "toast":
                        val = 2;
                        break;
                    default:
                        val = 3;
                        break;
                }
            }
            this.Headers["X-NotificationClass"] = ((int)_notificationClass + val).ToString();
        }

        /// <summary>
        /// Indicates that the notification should display a 'toast' message to the user. If the app
        /// is running, the message will be delivered to the app. Otherwise, it will be displayed in
        /// the notification bar and the user may expand it.
        /// </summary>
        /// <param name="text1">The first toast string (title).</param>
        /// <param name="text2">The second toast string (message).</param>
        /// <param name="param">An optional URI indicating where the user should be sent when they select the
        /// notification.</param>
        public void SetToast(string text1, string text2, string param = null)
        {
            this.Target = "toast";
            var sb = new StringBuilder();
            sb.AppendFormat("<wp:Toast><wp:Text1>{0}</wp:Text1><wp:Text2>{1}</wp:Text2>", text1, text2);
            if (param != null)
                sb.AppendFormat("<wp:Param>{0}</wp:Param>", param);
            sb.Append("</wp:Toast>");
            this.Body = string.Format(OuterFormat, sb.ToString());
        }

        /// <summary>
        /// Updates an application tile.
        /// </summary>
        /// <param name="count">The number to display on the tile, zero to remove the number, or null for no change.</param>
        /// <param name="title">Specifies the title to display on the tile, or null for no change.</param>
        /// <param name="backTitle">Sets the title for the back of the tile, or null for no change.</param>
        /// <param name="backContent">Specifies the text content to display on the back of the tile, or null for no change.</param>
        /// <param name="backgroundImage">Specifies the name of an image to show on the tile. This may be a file name, a URI
        /// referring to a valid image, or null for no change.</param>
        /// <param name="backBackgroundImage">Sets the background image for the back of the tile, or null for no change.</param>
        public void SetTile(int? count, string title = null, string backTitle = null, string backContent = null,
            string backgroundImage = null, string backBackgroundImage = null)
        {
            this.Target = "token";
            var sb = new StringBuilder();
            sb.Append("<wp:Tile>");
            if(count.HasValue)
                sb.AppendFormat("<wp:Count>{0}</wp:Count>", count);
            if(title != null)
                sb.AppendFormat("<wp:Title>{0}</wp:Title>", title);
            if (backTitle != null)
                sb.AppendFormat("<wp:BackTitle>{0}</wp:BackTitle>", backTitle);
            if (backContent != null)
                sb.AppendFormat("<wp:BackContent>{0}</wp:BackContent>", backContent);
            if(backgroundImage != null)
                sb.AppendFormat("<wp:BackgroundImage>{0}</wp:BackgroundImage>", backgroundImage);
            if(backBackgroundImage != null)
                sb.AppendFormat("<wp:BackBackgroundImage>{0}</wp:BackBackgroundImage>", backBackgroundImage);
            sb.Append("</wp:Tile>");
            this.Body = string.Format(OuterFormat, sb.ToString());
        }

        /// <summary>
        /// Delivers raw XML data to the application.
        /// </summary>
        /// <param name="rawXml">A string containing raw XML to deliver to the app.</param>
        public void SetRaw(string rawXml)
        {
            this.Target = null;
            this.Body = rawXml;
        }

        string INotificationPayload.GetPlatformName()
        {
            return "mpns";
        }
    }

    public enum WindowsPhoneNotificationClass
    {
        Realtime = 0,
        Priority = 10,
        Regular = 20
    }
}
