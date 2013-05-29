//     AppSecret.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography.X509Certificates;

namespace Piston.Push.Core
{
    public class AppSecret
    {
        [BsonElement("cert")]
        public byte[] Certificate { get; set; }

        [BsonElement("appId")]
        public string AppId { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }

        public X509Certificate2 GetX509Certificate()
        {
            if (this.Certificate == null)
                return null;
            else
                return new X509Certificate2(this.Certificate);
        }

        public X509Certificate2Collection GetX509Certificates()
        {
            if (this.Certificate == null)
                return null;
            else
                return new X509Certificate2Collection(new X509Certificate2[] { new X509Certificate2(this.Certificate) });
        }
    }
}
