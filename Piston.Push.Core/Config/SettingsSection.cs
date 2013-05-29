//     SettingsSection.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Configuration;

namespace Piston.Push.Core
{
    public class SettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("mongoStoreConnString", DefaultValue = "mongodb://localhost/?safe=true")]
        public string MongoStoreConnectionString
        {
            get { return (string) this["mongoStoreConnString"]; }
            set { this["mongoStoreConnString"] = value; }
        }

        [ConfigurationProperty("mongoDatabase", DefaultValue = "pushgw")]
        public string MongoDatabase
        {
            get { return (string) this["mongoDatabase"]; }
            set { this["mongoDatabase"] = value; }
        }

        [ConfigurationProperty("rabbitUri", DefaultValue = "amqp://guest:guest@localhost:5672/")]
        public string RabbitUri
        {
            get { return (string) this["rabbitUri"]; }
            set { this["rabbitUri"] = value; }
        }

        [ConfigurationProperty("apns")]
        public ApnsElement Apns
        {
            get { return (ApnsElement) this["apns"]; }
            set { this["apns"] = value; }
        }

        [ConfigurationProperty("mpns")]
        public MpnsElement Mpns
        {
            get { return (MpnsElement) this["mpns"]; }
            set { this["mpns"] = value; }
        }

        [ConfigurationProperty("gcm")]
        public GcmElement Gcm
        {
            get { return (GcmElement) this["gcm"]; }
            set { this["gcm"] = value; }
        }

        [ConfigurationProperty("wns")]
        public WnsElement Wns
        {
            get { return (WnsElement) this["wns"]; }
            set { this["wns"] = value; }
        }
    }
}
