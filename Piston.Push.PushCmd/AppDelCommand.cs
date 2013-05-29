//     AppDelCommand.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using Mono.Options;
using System.Collections.Generic;

namespace Piston.Push.PushCmd
{
    public class AppDelCommand : Command
    {
        public AppDelCommand()
            : base("app del", "Delete an application", "app add <appId>", 1)
        {
            this.Options = new OptionSet() {
            };
        }

        public override void Run(IList<string> args)
        {
            var store = new MongoMetaStore();
            store.Delete(args[0]);
        }
    }
}
