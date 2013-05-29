//     MongoTokenStore.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Piston.Push.Core
{
    public class MongoTokenStore : ITokenStore
    {
        private static readonly SettingsSection _settings = (SettingsSection)ConfigurationManager.GetSection("pushGateway");
        private static readonly MongoClient _client;
        private static readonly MongoServer _server;
        private static readonly MongoDatabase _db;

        static MongoTokenStore()
        {
            if (_settings == null)
                throw new ConfigurationErrorsException("MongoDB not configured.");
            _client = new MongoClient(_settings.MongoStoreConnectionString);
            _server = _client.GetServer();
            _db = _server.GetDatabase(_settings.MongoDatabase);
        }

        public void Register(string appId, Registration sub)
        {
            var coll = _db.GetCollection<Registration>(GetRegCollection(appId));
            sub.Updated = DateTime.Now.ToUnixEpoch();
            coll.Save(sub);
        }

        public IEnumerable<string> GetTokens(string appId, Platform platform, string segment)
        {
            var coll = _db.GetCollection<Registration>(GetRegCollection(appId));
            var query = Query.And(
                Query.EQ("segments", segment),
                Query.EQ("platform", platform)
                );
            return coll.Find(query).Select(r => r.Token);
        }

        public IEnumerable<Registration> FilterExisting(string appId, Platform platform, string[] deviceTokens)
        {
            var coll = _db.GetCollection<Registration>(GetRegCollection(appId));
            var query = Query.And(
                Query.In("_id", deviceTokens.Select(t => new BsonString(t))),
                Query.EQ("platform", platform)
                );
            return coll.Find(query);
        }

        public void UpdateRegistrationId(string appId, Platform platform, string oldDeviceToken, string newDeviceToken)
        {
            var coll = _db.GetCollection<Registration>(GetRegCollection(appId));
            var query = Query.And(
                    Query.EQ("_id", oldDeviceToken),
                    Query.EQ("platform", platform)
                    );
            var reg = coll.FindOne(query);
            if (reg != null)
            {
                reg.Token = newDeviceToken;
                coll.Save(reg);
                coll.Remove(query);
            }
        }

        public void Unregister(string appId, Platform platform, string deviceToken, uint? time)
        {
            var coll = _db.GetCollection<Registration>(GetRegCollection(appId));
            IMongoQuery query;
            if (!time.HasValue)
            {
                query = Query.And(
                    Query.EQ("_id", deviceToken),
                    Query.EQ("platform", platform)
                    );
            }
            else
            {
                query = Query.And(
                    Query.EQ("_id", deviceToken),
                    Query.EQ("platform", platform),
                    Query.LTE("updated", time)
                    );
            }
            coll.Remove(query);
        }

        private static string GetRegCollection(string appId)
        {
            return "regs." + appId;
        }
    }
}
