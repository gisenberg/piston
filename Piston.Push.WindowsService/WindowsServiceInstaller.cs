//     WindowsServiceInstaller.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

namespace Piston.Push.PushCmd
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        const string DisplayName = "Push Gateway Service";
        const string ServiceName = "PushGateway";

        public WindowsServiceInstaller()
        {
            var spi = new ServiceProcessInstaller();
            var si = new ServiceInstaller();

            spi.Account = ServiceAccount.LocalSystem;
            spi.Username = null;
            spi.Password = null;

            si.DisplayName = DisplayName;
            si.StartType = ServiceStartMode.Automatic;
            si.ServiceName = ServiceName;

            this.Installers.Add(spi);
            this.Installers.Add(si);
        }
    }
}
