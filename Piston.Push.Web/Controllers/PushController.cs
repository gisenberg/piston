//     PushController.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using System;
using System.Linq;
using System.Web.Http;

namespace Piston.Push.Web
{
    [RestHandleError]
    public class PushController : ApiController
    {
        ITokenStore _store;
        INotificationPublisher _queue;

        public PushController(ITokenStore store, INotificationPublisher queue)
        {
            _store = store;
            _queue = queue;
        }

        [HttpPost]
        public object Register(Registration reg)
        {
            string appId = (string)this.Request.Properties["AppId"];
            reg.Validate();
            _store.Register(appId, reg);
            return new { success = true };
        }

        [HttpPost]
        public object Unregister(Registration reg)
        {
            string appId = (string)this.Request.Properties["AppId"];
            _store.Unregister(appId, reg.Platform, reg.Token, null);
            return new { success = true };
        }

        [HttpPost]
        public object Push(NotificationRequest noteRequest)
        {
            string appId = (string)this.Request.Properties["AppId"];
            if (noteRequest == null)
                throw new ArgumentNullException("noteRequest");
            noteRequest.Validate();

            int count = 0;

            // enqueue individual tokens
            string[] goodTokens = new string[0];
            if (noteRequest.Tokens != null)
            {
                var store = new MongoTokenStore();
                goodTokens = store.FilterExisting(appId, noteRequest.Platform, noteRequest.Tokens).Select(r => r.Token).Distinct().ToArray();
                foreach (var tok in goodTokens)
                {
                    count++;
                    _queue.Enqueue(appId, noteRequest.Platform, tok, noteRequest.Expiry, noteRequest.Payload);
                }
            }

            // enqueue segment groups, except tokens already specified
            if (noteRequest.Segments != null)
            {
                foreach (var seg in noteRequest.Segments)
                {
                    foreach (var tok in _store.GetTokens(appId, noteRequest.Platform, seg).Except(goodTokens))
                    {
                        count++;
                        _queue.Enqueue(appId, noteRequest.Platform, tok, noteRequest.Expiry, noteRequest.Payload);
                    }
                }
            }

            return new { success = true, count = count, badTokens = noteRequest.Tokens.Except(goodTokens).ToArray() };
        }
    }
}
