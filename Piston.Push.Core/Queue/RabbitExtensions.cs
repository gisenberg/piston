//     RabbitExtensions.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using RabbitMQ.Client;

namespace Piston.Push.Core
{
    public static class RabbitExtensions
    {
        public static string GetStringHeader(this IBasicProperties props, string headerName)
        {
            if (!props.IsHeadersPresent()
                || !props.Headers.Contains(headerName))
                return null;

            object obj = props.Headers[headerName];
            if (obj is string)
                return (string)obj;
            if (obj is byte[])
                return System.Text.Encoding.UTF8.GetString((byte[])obj);
            return null;
        }
    }
}
