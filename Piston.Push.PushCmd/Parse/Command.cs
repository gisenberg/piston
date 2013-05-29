//     Command.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piston.Push.PushCmd
{
    public abstract class Command
    {
        public static readonly Command[] Commands = Collect();

        public static Command[] Collect()
        {
            return (from t in Assembly.GetCallingAssembly().GetTypes()
                    where t.IsSubclassOf(typeof(Command)) && !t.IsAbstract
                    select (Command)Activator.CreateInstance(t))
                    .OrderBy(c => c.CommandName).ToArray();
        }

        public static int Usage(Command command = null, string message = null)
        {
            var exeName = typeof(Program).Assembly.GetName().Name;
            if (message != null)
                Console.WriteLine(message);
            Console.WriteLine(string.Format("Usage: {0} {1}{2}", exeName,
                command == null ? "<command> [--help]" : command.CommandUsage,
                command != null && command.Options != null ? " [options]" : ""));
            if (command == null)
            {
                Console.WriteLine("Commands:");
                foreach (var c in Commands)
                    Console.WriteLine("\t{0,-16}{1}", c.CommandName, c.CommandDescription);
            }
            else if (command.Options != null)
            {
                command.Options.WriteOptionDescriptions(Console.Out);
            }
            return -1;
        }

        public static int Process(string[] args)
        {
            if (args.Length < 1)
                return Usage();

            var op = (from c in Commands
                      let parts = c.CommandName.Split(' ')
                      where args.Length >= parts.Length
                      && args.Take(parts.Length).SequenceEqual(parts, StringComparer.OrdinalIgnoreCase)
                      select new { cmd = c, args = args.Skip(parts.Length).ToArray() }).FirstOrDefault();
            if (op == null)
                return Usage();

            if (op.cmd.Options != null)
            {
                try
                {
                    op = new { cmd = op.cmd, args = op.cmd.Options.Parse(op.args).ToArray() };
                }
                catch (Exception ex)
                {
                    return Usage(op.cmd, ex.Message);
                }
            }

            if (op.args.Contains("--help") || op.args.Length < op.cmd.MinArgCount)
            {
                return Usage(op.cmd);
            }

            try
            {
                op.cmd.Run(op.args);
                return 0;
            }
            catch (UsageException ex)
            {
                return Usage(op.cmd, ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public Command(string name, string desc, string usage, int minArgCount = 0)
        {
            this.CommandName = name;
            this.CommandDescription = desc;
            this.CommandUsage = usage;
            this.MinArgCount = minArgCount;
        }

        public string CommandName { get; private set; }
        public string CommandUsage { get; private set; }
        public int MinArgCount { get; private set; }
        public string CommandDescription { get; private set; }
        public OptionSet Options { get; protected set; }

        public abstract void Run(IList<string> args);
    }
}
