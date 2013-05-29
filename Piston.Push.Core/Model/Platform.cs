//     Platform.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

namespace Piston.Push.Core
{
    public enum Platform
    {
        Apns = 1, // Apple (iOS)
        Gcm = 2, // Google (Android)
        Mpns = 3, // Microsoft (Windows Phone)
        Wns = 4, // Microsoft (Windows 8 Store)
    }
}
