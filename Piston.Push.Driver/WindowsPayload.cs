//     WindowsPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Holds data specific to push notifications sent to Windows Store apps.
    /// </summary>
    public sealed class WindowsPayload : INotificationPayload
    {
        const string OuterFormat = "<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}";

        public WindowsPayload()
        {
            this.Headers = new Dictionary<string, string>();
            this.SetRaw("");
        }

        [JsonProperty("headers")]
        internal Dictionary<string, string> Headers { get; private set; }

        [JsonProperty("body")]
        internal string Body { get; set; }

        [JsonIgnore]
        internal string NotifcationType
        {
            get
            {
                return this.Headers["X-WNS-Type"];
            }
            set
            {
                if (value == null && this.Headers.ContainsKey("X-WNS-Type"))
                    this.Headers.Remove("X-WNS-Type");
                else
                    this.Headers["X-WNS-Type"] = value;
            }
        }

        /// <summary>
        /// Indicates that the notification should display a 'toast' message to the user. If the app
        /// is running, the message will be delivered to the app. Otherwise, it will be displayed in
        /// the notification bar and the user may expand it.
        /// </summary>
        /// <param name="text1">The first toast string (title).</param>
        public void SetToast(string text1)
        {
            this.NotifcationType = "wns/toast";
            var sb = new StringBuilder();
            sb.AppendFormat("<toast><visual version='1'><binding template='ToastText01'><text id='1'>{0}</text></binding></visual></toast>",
                text1);
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
            this.NotifcationType = "wns/tile";
            var sb = new StringBuilder();
            sb.Append("<wp:Tile>");
            if (count.HasValue)
                sb.AppendFormat("<wp:Count>{0}</wp:Count>", count);
            if (title != null)
                sb.AppendFormat("<wp:Title>{0}</wp:Title>", title);
            if (backTitle != null)
                sb.AppendFormat("<wp:BackTitle>{0}</wp:BackTitle>", backTitle);
            if (backContent != null)
                sb.AppendFormat("<wp:BackContent>{0}</wp:BackContent>", backContent);
            if (backgroundImage != null)
                sb.AppendFormat("<wp:BackgroundImage>{0}</wp:BackgroundImage>", backgroundImage);
            if (backBackgroundImage != null)
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
            this.NotifcationType = "wns/raw";
            this.Body = rawXml;
        }

        string INotificationPayload.GetPlatformName()
        {
            return "wns";
        }
    }
}
