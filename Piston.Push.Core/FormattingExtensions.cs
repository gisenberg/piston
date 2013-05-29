//     FormattingExtensions.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.Text;

namespace Piston.Push.Core
{
    public static class FormattingExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return ToHexString(bytes, 0, bytes.Length);
        }

        public static string ToHexString(this byte[] bytes, int index, int count)
        {
            var sb = new StringBuilder(count);
            for (var i = index; i < index + count; i++)
            {
                sb.AppendFormat("{0:X2}", bytes[i]);
            }
            return sb.ToString();
        }

        public static byte[] FromHexString(string data)
        {
            var numBytes = data.Length / 2;
            var bytes = new byte[numBytes];
            FromHexString(data, bytes, 0);
            return bytes;
        }

        public static void FromHexString(string data, byte[] buf, int index)
        {
            var numBytes = data.Length / 2;
            if (data.Length % 2 != 0)
                throw new FormatException("String length must be even.");

            for (var i = 0; i < numBytes; i++)
            {
                buf[index + i] = (byte)((HexDigitToNumber(data[i * 2]) << 4) + HexDigitToNumber(data[i * 2 + 1]));
            }
        }

        private static int HexDigitToNumber(char c)
        {
            int val;
            if (c >= '0' && c <= '9')
                val = c - '0';
            else if (c >= 'A' && c <= 'F')
                val = (c - 'A') + 10;
            else if (c >= 'a' && c <= 'f')
                val = (c - 'a') + 10;
            else
                throw new FormatException("Invalid hex character.");
            return val;
        }

        public static void ToBytesNetworkOrder(this int val, byte[] buf, int index)
        {
            val = System.Net.IPAddress.HostToNetworkOrder(val);
            var bytes = BitConverter.GetBytes(val);
            for (var i = 0; i < 4; i++)
                buf[index + i] = bytes[i];
        }

        public static void ToBytesNetworkOrder(this uint val, byte[] buf, int index)
        {
            ToBytesNetworkOrder((int)val, buf, index);
        }

        public static void ToBytesNetworkOrder(this short val, byte[] buf, int index)
        {
            val = System.Net.IPAddress.HostToNetworkOrder(val);
            var bytes = BitConverter.GetBytes(val);
            buf[index] = bytes[0];
            buf[index + 1] = bytes[1];
        }

        public static void ToBytesNetworkOrder(this ushort val, byte[] buf, int index)
        {
            ToBytesNetworkOrder((short)val, buf, index);
        }

        public static int FromBytesNetworkOrder(byte[] buf, int index)
        {
            var result = BitConverter.ToInt32(buf, index);
            return System.Net.IPAddress.NetworkToHostOrder(result);
        }

        public static uint ToUnixEpoch(this DateTime dt)
        {
            return (uint)(dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
