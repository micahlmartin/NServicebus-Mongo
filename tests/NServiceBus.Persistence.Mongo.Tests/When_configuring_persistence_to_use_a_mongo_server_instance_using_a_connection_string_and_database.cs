using MongoDB.Driver;
using NUnit.Framework;
using System;

namespace NServiceBus.Persistence.Mongo.Tests
{
    [TestFixture]
    public class When_configuring_persistence_to_use_a_mongo_server_instance_using_a_connection_string_and_database
    {
        string _connectionStringName;
        string _database;
        MongoDatabase _db;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _connectionStringName = "MongoDB";
            _database = "CustomDatabase";

            var config = Configure.With(new[] { GetType().Assembly })
                .DefineEndpointName("UnitTests")
                .DefaultBuilder()
                .MongoPersistence(_connectionStringName, _database);

            _db = config.Builder.Build<MongoDatabase>();
        }

        [Test]
        public void It_should_use_a_document_store()
        {
            Assert.IsNotNull(_db);
        }

        [Test]
        public void It_should_configure_the_document_store_with_the_connection_string()
        {
            Assert.AreEqual("127.0.0.1", _db.Server.Settings.Server.Host);
            Assert.AreEqual(27017, _db.Server.Settings.Server.Port);
        }

        [Test]
        public void It_should_configure_the_document_store_to_use_the_database()
        {
            Assert.AreEqual(_database, _db.Name);
        }
    }

    [TestFixture]
    public class When_configuring_the_mongo_persister_with_a_connection_string_that_has_a_default_database_set
    {
        string _connectionStringName;

        MongoDatabase _db;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _connectionStringName = "MongoDBWithDatabase";

            var config = Configure.With(new[] { GetType().Assembly })
                .DefineEndpointName("UnitTests")
                .DefaultBuilder()
                .MongoPersistence(_connectionStringName);

            _db = config.Builder.Build<MongoDatabase>();
        }


        [Test]
        public void It_should_use_the_default_database_of_the_store()
        {
            Assert.AreEqual("MyDB", _db.Name);
        }
    }
}