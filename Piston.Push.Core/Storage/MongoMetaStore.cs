//     MongoMetaStore.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using MongoDB.Driver;
using System.Collections.Generic;
using System.Configuration;

namespace Piston.Push.Core
{
    public class MongoMetaStore : IMetaStore
    {
        const string AppCollection = "meta.apps";
        private static readonly SettingsSection _settings = (SettingsSection)ConfigurationManager.GetSection("pushGateway");
        private static readonly MongoClient _client;
        private static readonly MongoServer _server;
        private static readonly MongoDatabase _db;

        static MongoMetaStore()
        {
            if (_settings == null)
                throw new ConfigurationErrorsException("MongoDB not configured.");
            _client = new MongoClient(_settings.MongoStoreConnectionString);
            _server = _client.GetServer();
            _db = _server.GetDatabase(_settings.MongoDatabase);
        }

        public AppMetadata GetAppById(string appId)
        {
            var coll = _db.GetCollection<AppMetadata>(AppCollection);
            return coll.FindOne(new QueryDocument("_id", appId));
        }

        public AppMetadata GetAppByKey(string apiKey)
        {
            var coll = _db.GetCollection<AppMetadata>(AppCollection);
            return coll.FindOne(new QueryDocument("apiKey", apiKey));
        }

        public void Save(AppMetadata app)
        {
            var coll = _db.GetCollection<AppMetadata>(AppCollection);
            coll.Save(app);
        }

        public void Delete(string appId)
        {
            var coll = _db.GetCollection<AppMetadata>(AppCollection);
            coll.Remove(new QueryDocument("_id", appId));
        }

        public IEnumerable<AppMetadata> GetApps()
        {
            var coll = _db.GetCollection<AppMetadata>(AppCollection);
            return coll.FindAll();
        }
    }
}
