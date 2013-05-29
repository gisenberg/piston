//     GcmPayload.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Piston.Push.Core
{
    public class GcmPayload
    {
        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }

        [JsonProperty("collapse_key")]
        public string CollapseKey { get; set; }

        [JsonProperty("delay_while_idle")]
        public bool? DelayWhileIdle { get; set; }

        [JsonProperty("time_to_live")]
        public uint? TimeToLive { get; set; }

        [JsonProperty("restricted_package_name")]
        public bool? RestrictedPackageName { get; set; }

        [JsonProperty("dry_run")]
        public bool? DryRun { get; set; }

        [JsonProperty("data")]
        public JObject Data { get; set; }
    }
}
