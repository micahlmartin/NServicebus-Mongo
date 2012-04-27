using System.Linq;
using NServiceBus.Persistence.Mongo;
using NUnit.Framework;

namespace NServiceBus.Unicast.Subscriptions.Mongo.Tests
{
    [TestFixture]
    public class When_initializing_the_storage_with_existing_v2X_subscriptions : MongoFixture
    {
        [Test]
        public void Should_automatically_update_them_to_the_30_format()
        {
            Database.GetCollection(MongoPersistenceConstants.SubscriptionCollectionName).Insert(new
                                                                                                    {
                                                                                                        SubscriberEndpoint = TestClients.ClientA.ToString(),
                                                                                                        MessageType = typeof(MessageB).AssemblyQualifiedName
                                                                                                    });
            
            Storage.Init();

            var subscriptionsForMessageType = Storage.GetSubscriberAddressesForMessage(MessageTypes.MessageB);

            Assert.AreEqual(1, subscriptionsForMessageType.Count());
            Assert.AreEqual(subscriptionsForMessageType.First(), TestClients.ClientA);
        }
    }
}