using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using NServiceBus.Persistence.Mongo;
using NServiceBus.Saga;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.Mongo.Tests
{
    public class MongoFixture
    {
        private MongoDatabase _database;
        private ISagaPersister _sagaPersister;
        private MongoCollection<BsonDocument> _sagas;
        private MongoClient _client;

        [SetUp]
        public virtual void SetupContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;

            _client = new MongoClient(connectionString);
            _database = _client.GetServer().GetDatabase("Test_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            _sagaPersister = new MongoSagaPersister(_database);
            _sagas = _database.GetCollection(MongoPersistenceConstants.SagaCollectionName);
        }

        protected MongoCollection<BsonDocument> Sagas
        {
            get { return _database.GetCollection<BsonDocument>(MongoPersistenceConstants.SagaCollectionName); }
        }

        protected MongoDatabase Database
        {
            get { return _database; }
        }

        protected ISagaPersister SagaPersister
        {
            get { return _sagaPersister; }
        }

        [TearDown]
        public void TeardownContext()
        {
            _database.Drop();
        }

        protected void SaveSaga<T>(T saga) where T : IContainSagaData
        {
            _sagaPersister.Save(saga);
        }

        protected void CompleteSaga<T>(Guid sagaId) where T : IContainSagaData
        {
            var saga = _sagaPersister.Get<T>(sagaId);
            Assert.NotNull(saga);
            _sagaPersister.Complete(saga);
        }

        protected void UpdateSaga<T>(Guid sagaId, Action<T> update) where T : IContainSagaData
        {
            var saga = _sagaPersister.Get<T>(sagaId);
            Assert.NotNull(saga, "Could not update saga. Saga not found");
            update(saga);
            _sagaPersister.Update(saga);
        }
    }
}