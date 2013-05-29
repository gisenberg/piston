//     Registration.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Piston.Push.Core
{
    public class Registration
    {
        [JsonProperty("platform")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonElement("platform")]
        public Platform Platform { get; set; }

        [BsonId]
        [JsonProperty("token")]
        public string Token { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("segments")]
        [JsonProperty("segments")]
        public string[] Segments { get; set; }

        [JsonIgnore]
        [BsonElement("updated")]
        public uint Updated { get; set; }

        public void Validate()
        {
            DeviceToken.Validate(this.Platform, this.Token);
            this.Token = DeviceToken.Normalize(this.Platform, this.Token);
        }
    }
}
