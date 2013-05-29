//     AppListCommand.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using Mono.Options;
using System;
using System.Collections.Generic;

namespace Piston.Push.PushCmd
{
    public class AppListCommand : Command
    {
        public AppListCommand()
            : base("app list", "List all apps", "app list", 0)
        {
            this.Options = new OptionSet() { };
        }

        public override void Run(IList<string> args)
        {
            var store = new MongoMetaStore();
            foreach (var app in store.GetApps())
            {
                Console.WriteLine(string.Format("{0} - API Key: {1}", app.AppId, app.ApiKey));
            }
        }
    }
}
