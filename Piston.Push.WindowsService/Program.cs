//     Program.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.IO;

namespace Piston.Push.Service
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                using (var log = new StreamWriter("install.log", false))
                {
                    if (args[0] == "/i")
                    {
                        var exePath = Assembly.GetExecutingAssembly().Location;
                        log.WriteLine("Installing service at " + exePath);
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] {exePath});
                        }
                        catch (Exception ex)
                        {
                            log.WriteLine(ex.ToString());
                        }
                    }
                    else if (args[0] == "/u")
                    {
                        var exePath = Assembly.GetExecutingAssembly().Location;
                        log.WriteLine("Installing service at " + exePath);
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] {"/u", exePath});
                        }
                        catch (Exception ex)
                        {
                            log.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else
            {
                var services = new ServiceBase[] { new WindowsService() };
                ServiceBase.Run(services);
            }
        }
    }
}
