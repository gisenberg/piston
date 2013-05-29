//     ServerCommand.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Core;
using Mono.Options;
using System.Collections.Generic;
using System.Diagnostics;

namespace Piston.Push.PushCmd
{
    public class ServerCommand : Command
    {
        bool _trace = false;

        public ServerCommand()
            : base("server", "Start push server", "server", 0)
        {
            this.Options = new OptionSet() {
                { "trace", "show trace information", val => _trace = val != null }
            };
        }

        public override void Run(IList<string> args)
        {
            if (_trace)
            {
                Trace.Listeners.Add(new ConsoleTraceListener());
            }

            var server = new Server();
            server.Start();
        }
    }
}
