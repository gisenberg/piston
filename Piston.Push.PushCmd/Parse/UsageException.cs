//     UsageException.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.PushCmd
{
    public class UsageException : Exception
    {
        public UsageException(string message) : base(message) { }
    }
}
