//     SettingsSection.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Configuration;

namespace Piston.Push.Driver
{
    public class SettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", DefaultValue = "http://localhost:54585/")]
        public string BaseUrl
        {
            get { return (string)this["baseUrl"]; }
            set { this["baseUrl"] = value; }
        }

        [ConfigurationProperty("appId")]
        public string AppId
        {
            get { return (string)this["appId"]; }
            set { this["appId"] = value; }
        }

        [ConfigurationProperty("apiKey")]
        public string ApiKey
        {
            get { return (string)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }
    }
}
