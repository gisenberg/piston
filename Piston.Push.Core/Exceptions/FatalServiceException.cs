//     FatalServiceException.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;

namespace Piston.Push.Core
{
    public class FatalServiceException : Exception
    {
        public FatalServiceException(string message)
            : base(message)
        {
        }

        public FatalServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
