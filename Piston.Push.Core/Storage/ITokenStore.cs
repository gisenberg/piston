//     ITokenStore.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Collections.Generic;

namespace Piston.Push.Core
{
    public interface ITokenStore
    {
        void Register(string appId, Registration device);
        IEnumerable<string> GetTokens(string appId, Platform platform, string segment);
        IEnumerable<Registration> FilterExisting(string appId, Platform platform, string[] deviceTokens);
        void UpdateRegistrationId(string appId, Platform platform, string oldDeviceToken, string newDeviceToken);
        void Unregister(string appId, Platform platform, string deviceToken, uint? time);
    }
}
