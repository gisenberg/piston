//     AppAddCommand.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Piston.Push.PushCmd
{
    public class AppAddCommand : Command
    {
        const int ApiKeySizeBytes = 32;

        string _apnsCertFile, _mpnsCertFile, _gcmKey, _wnsKey;

        public AppAddCommand()
            : base("app add", "Create an application", "app add <appId>", 1)
        {
            this.Options = new OptionSet() {
                { "apns=", "specify path to Apple certificate", val => _apnsCertFile = val },
                { "mpns=", "specify optional path to Microsoft certificate", val => _mpnsCertFile = val },
                { "gcm=", "specify Google app key", val => _gcmKey = val },
                { "wns=", "specify SID:secret pair", val => _wnsKey = val }
            };
        }

        public override void Run(IList<string> args)
        {
            var store = new MongoMetaStore();
            var appMetadata = store.GetAppById(args[0]);
            if (appMetadata == null)
                appMetadata = new AppMetadata { AppId = args[0], Platforms = new Dictionary<string, AppSecret>() };

            if (string.IsNullOrEmpty(appMetadata.ApiKey))
            {
                appMetadata.ApiKey = GenerateApiKey();
            }

            if (!string.IsNullOrWhiteSpace(_apnsCertFile))
            {
                appMetadata.Platforms["apns"] = new AppSecret { Certificate = File.ReadAllBytes(_apnsCertFile) };
            }
            if (_mpnsCertFile != null)
            {
                appMetadata.Platforms["mpns"] = new AppSecret
                {
                    Certificate = string.IsNullOrWhiteSpace(_mpnsCertFile) ? null : File.ReadAllBytes(_mpnsCertFile)
                };
            }
            if (!string.IsNullOrWhiteSpace(_gcmKey))
            {
                appMetadata.Platforms["gcm"] = new AppSecret { Key = _gcmKey };
            }
            if (!string.IsNullOrEmpty(_wnsKey))
            {
                var parts = _wnsKey.Split('?');
                Console.WriteLine(parts.Length);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                    throw new ArgumentException("WNS credentials must be in the format SID?secret");
                appMetadata.Platforms["wns"] = new AppSecret {AppId = parts[0], Key = parts[1]};
            }
            store.Save(appMetadata);

            Console.WriteLine("API Key: " + appMetadata.ApiKey);
        }

        private static string GenerateApiKey()
        {
            var rng = RandomNumberGenerator.Create();
            var buf = new byte[ApiKeySizeBytes];
            rng.GetBytes(buf);
            return Convert.ToBase64String(buf);
        }
    }
}
