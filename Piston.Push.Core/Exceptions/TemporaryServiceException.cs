//     TemporaryServiceException.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Core
{
    public class TemporaryServiceException : Exception
    {
        public TemporaryServiceException(string message)
            : base(message)
        {
        }

        public TemporaryServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
