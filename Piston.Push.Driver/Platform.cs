//     Platform.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Driver
{
    /// <summary>
    /// Platforms targeted by push notifications.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Apple iOS devices, including iPad, iPhone, and iPod.
        /// </summary>
        iOS,
        /// <summary>
        /// Google Android devices.
        /// </summary>
        Android,
        /// <summary>
        /// Microsoft Windows Phone devices and Windows 8 tablets.
        /// </summary>
        WindowsPhone,
        /// <summary>
        /// Microsoft Windows 8 desktop.
        /// </summary>
        Windows
    }

    internal static class PlatformExtensions
    {
        public static string ToPlatformName(this Platform platform)
        {
            switch (platform)
            {
                case Platform.iOS:
                    return "apns";
                case Platform.Android:
                    return "gcm";
                case Platform.WindowsPhone:
                    return "mpns";
                case Platform.Windows:
                    return "wns";
                default:
                    throw new ArgumentException("Invalid platform");
            }
        }
    }
}
