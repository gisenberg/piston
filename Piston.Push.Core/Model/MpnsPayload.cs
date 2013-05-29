//     MpnsPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Piston.Push.Core
{
    public class MpnsPayload
    {
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
