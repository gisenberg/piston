//     INotificationPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

namespace Piston.Push.Driver
{
    public interface INotificationPayload
    {
        string GetPlatformName();
    }
}
