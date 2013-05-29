//     NotificationRequest.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Piston.Push.Core
{
    public class NotificationRequest
    {
        const int MaxApplePayloadSize = 256;

        [JsonProperty("platform")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [JsonProperty("tokens")]
        public string[] Tokens { get; set; }

        [JsonProperty("segments")]
        public string[] Segments { get; set; }

        [JsonProperty("expiry")]
        public uint Expiry { get; set; }

        [JsonProperty("payload")]
        public JObject Payload { get; set; }

        public void Validate()
        {
            if (this.Tokens != null)
            {
                foreach (var tok in this.Tokens)
                    DeviceToken.Validate(this.Platform, tok);
            }
            if (this.Segments != null)
            {
                foreach (var seg in this.Segments)
                    if (string.IsNullOrWhiteSpace(seg))
                        throw new ValidationException("Invalid segment.");
            }

            switch (this.Platform)
            {
                case Platform.Gcm:
                    break;
                case Platform.Mpns:
                    {
                        var payload = this.Payload.ToObject<MpnsPayload>();
                        if (payload.Headers == null)
                            throw new ValidationException("Headers collection required");
                        foreach (var header in payload.Headers)
                        {
                            if (header.Value == null || !header.Key.StartsWith("X-"))
                                throw new ValidationException("Invalid custom headers.");
                        }
                        if (string.IsNullOrEmpty(payload.Body))
                            throw new ValidationException("Message body is required.");
                    }
                    break;
                case Core.Platform.Wns:
                    break;
                case Platform.Apns:
                    {
                        var payload = JsonConvert.SerializeObject(this.Payload);
                        if (payload.Length > MaxApplePayloadSize)
                            throw new ValidationException("Payload too large.");
                    }
                    break;
                default:
                    throw new ValidationException("Invalid platform.");
            }

            if (this.Platform == 0)
                throw new ValidationException("Platform is required.");
        }
    }
}
