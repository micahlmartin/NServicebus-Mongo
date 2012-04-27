using NUnit.Framework;
using MongoDB.Driver.Linq;

namespace NServiceBus.Unicast.Subscriptions.Mongo.Tests
{
    [TestFixture]
    public class When_receiving_a_subscription_message : MongoFixture
    {
        [Test]
        public void A_subscription_entry_should_be_added_to_the_database()
        {
            var messageTypes = new[] {new MessageType(typeof (MessageA)), new MessageType(typeof (MessageB))};

            Storage.Subscribe(TestClients.ClientA, messageTypes);

            var count = Subscriptions.Count();

            Assert.AreEqual(count, 2);
        }

        [Test]
        public void Duplicate_subcription_shouldnt_create_additional_db_rows()
        {

            Storage.Subscribe(TestClients.ClientA, MessageTypes.MessageA);
            Storage.Subscribe(TestClients.ClientA, MessageTypes.MessageA);


            var count = Subscriptions.Count();
            Assert.AreEqual(count, 1);
        }
    }
}
