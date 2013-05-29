//     TemporaryUndeliverableException.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Core
{
    public class TemporaryUndeliverableException : Exception
    {
        public TemporaryUndeliverableException(string message)
            : base(message)
        {
        }

        public TemporaryUndeliverableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
