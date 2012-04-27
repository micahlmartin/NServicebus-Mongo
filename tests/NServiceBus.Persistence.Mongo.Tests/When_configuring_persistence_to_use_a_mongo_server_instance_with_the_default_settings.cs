using MongoDB.Driver;
using NUnit.Framework;

namespace NServiceBus.Persistence.Mongo.Tests
{
    [TestFixture]
    public class When_configuring_persistence_to_use_a_mongo_server_instance_with_the_default_settings
    {
        MongoDatabase _db;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var config = Configure.With(new[] {GetType().Assembly})
                .DefineEndpointName("UnitTests")
                .DefaultBuilder()
                .MongoPersistence();

            _db = config.Builder.Build<MongoDatabase>();
        }

        [Test]
        public void It_should_configure_the_document_store_to_use_the_default_url()
        {
            Assert.AreEqual("127.0.0.1", _db.Server.Settings.Server.Host);
            Assert.AreEqual(27017, _db.Server.Settings.Server.Port);
        }

        [Test]
        public void It_should_configure_the_document_store_to_use_the_calling_assembly_name_as_the_database()
        {

            Assert.AreEqual("UnitTests", _db.Name);
        }
    }
}