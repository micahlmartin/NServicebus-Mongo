using System.Configuration;
using NUnit.Framework;

namespace NServiceBus.Persistence.Mongo.Tests
{
    [TestFixture]
    public class When_configuring_persistence_to_use_a_mongo_server_instance_using_a_connection_string_that_does_not_exist
    {
        [Test]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void It_should_throw_an_exception()
        {
            Configure.With(new[] { GetType().Assembly })
                .DefaultBuilder()
                .MongoPersistence("ConnectionStringDoesNotExist");
        }
    }
}