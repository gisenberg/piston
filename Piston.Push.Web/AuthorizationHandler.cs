//     AuthorizationHandler.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Piston.Push.Web
{
    public class AuthorizationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string appId, apiKey;
            if (IsAuthValid(request, out appId, out apiKey))
            {
                request.Properties["AppId"] = appId;
                return base.SendAsync(request, cancellationToken);
            }
            else
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                    new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Bad authorization.") });
            }
        }

        private static bool IsAuthValid(HttpRequestMessage request, out string appId, out string apiKey)
        {
            IEnumerable<string> authValues;
            appId = null;
            apiKey = null;
            if (!request.Headers.TryGetValues("Authorization", out authValues))
                return false;
            foreach (var authStr in authValues)
            {
                if (string.IsNullOrWhiteSpace(authStr))
                    continue;
                var partsSpace = authStr.Split(null, 2);
                if (string.Compare("APIKEY", partsSpace[0], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    foreach (var pair in partsSpace[1].Split(',').Select(p => p.Trim()))
                    {
                        var partsEquals = pair.Split(new char[] { '=' }, 2).Select(p => p.Trim()).ToArray();
                        if (partsEquals.Length == 2)
                        {
                            if (string.Compare("appId", partsEquals[0], StringComparison.OrdinalIgnoreCase) == 0)
                                appId = partsEquals[1].Trim();
                            else if (string.Compare("apiKey", partsEquals[0], StringComparison.OrdinalIgnoreCase) == 0)
                                apiKey = partsEquals[1].Trim().Replace("\"", "");
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(apiKey))
                return false;

            var store = new MongoMetaStore();
            var appMetadata = store.GetAppById(appId);
            return appMetadata != null && appMetadata.ApiKey == apiKey;
        }
    }
}
