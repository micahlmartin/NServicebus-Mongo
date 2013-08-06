using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using NServiceBus.Persistence.Mongo;
using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;
using NUnit.Framework;

namespace NServiceBus.Unicast.Subscriptions.Mongo.Tests
{
    public class MongoFixture
    {
        private ISubscriptionStorage _storage;
        private MongoDatabase _database;
        private MongoClient _client;

        [SetUp]
        public void SetupContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;

            _client = new MongoClient(connectionString);;
            _database = _client.GetServer().GetDatabase("Test_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            _storage = new SubscriptionStorage(_database);
        }

        protected ISubscriptionStorage Storage
        {
            get { return _storage; }
        }

        protected MongoCollection<Subscription> Subscriptions
        {
            get { return _database.GetCollection<Subscription>(MongoPersistenceConstants.SubscriptionCollectionName); }
        }

        protected MongoDatabase Database
        {
            get { return _database; }
        }

        [TearDown]
        public void TeardownContext()
        {
            _database.Drop();
        }
    }
}