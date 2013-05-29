//     DeviceToken.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

namespace Piston.Push.Core
{
    public static class DeviceToken
    {
        public static void Validate(Platform platform, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ValidationException("Missing or invalid token.");

            switch (platform)
            {
                case Platform.Gcm:
                    break;
                case Platform.Mpns:
                    break;
                case Platform.Apns:
                    var bytes = FormattingExtensions.FromHexString(token);
                    if (bytes.Length != 32)
                        throw new ValidationException("Invalid device token.");
                    break;
                case Platform.Wns:
                    break;
                default:
                    throw new ValidationException("Invalid platform.");
            }
        }

        public static string Normalize(Platform platform, string token)
        {
            switch (platform)
            {
                case Platform.Apns:
                    return token.ToUpper();
                default:
                    return token;
            }
        }
    }
}
