//     AppMetadata.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Piston.Push.Core
{
    public class AppMetadata
    {
        [BsonId]
        public string AppId { get; set; }

        [BsonElement("apiKey")]
        public string ApiKey { get; set; }

        [BsonElement("platforms")]
        public Dictionary<string, AppSecret> Platforms { get; set; }
    }
}
