//     INotificationConsumer.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Core
{
    public interface INotificationConsumer : IDisposable
    {
        NotificationEventArgs Dequeue();
        void Accept(NotificationEventArgs note);
        void Reject(NotificationEventArgs note, bool requeue);
        string AppId { get; }
        Platform Platform { get; }
    }
}
