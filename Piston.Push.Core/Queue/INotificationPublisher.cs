//     INotificationPublisher.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

namespace Piston.Push.Core
{
    public interface INotificationPublisher
    {
        void Enqueue(string appId, Platform platform, string deviceToken, uint expiry, object payload);
    }
}
