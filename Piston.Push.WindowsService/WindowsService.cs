//     WindowsService.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.ServiceProcess;
using Piston.Push.Core;

namespace Piston.Push.Service
{
    public class WindowsService : ServiceBase
    {
        public const string PushGatewayServiceName = "PushGateway";

        Server _server;

        public WindowsService()
        {
            this.ServiceName = PushGatewayServiceName;
            this.EventLog.Log = "Application";

            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = false;
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            _server = new Server();
            _server.Start();
        }

        protected override void OnStop()
        {
            _server.Dispose();

            base.OnStop();
        }
    }
}
