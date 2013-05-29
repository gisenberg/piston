//     UndeliverableException.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Core
{
    public class UndeliverableException : Exception
    {
        public UndeliverableException(string message)
            : base(message)
        {
        }

        public UndeliverableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
