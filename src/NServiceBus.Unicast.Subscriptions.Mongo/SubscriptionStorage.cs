using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using NServiceBus.Persistence.Mongo;
using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;
using NServiceBus.Logging;

namespace NServiceBus.Unicast.Subscriptions.Mongo
{
    public class SubscriptionStorage : ISubscriptionStorage
    {
        private readonly MongoCollection<Subscription> _subscriptions;

        public SubscriptionStorage(MongoDatabase database)
        {
            _subscriptions = database.GetCollection<Subscription>(MongoPersistenceConstants.SubscriptionCollectionName);
        }

        public void Subscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            foreach (var messageType in messageTypes)
            {
                if(_subscriptions.AsQueryable().Where(x => x.TypeName == messageType.TypeName && x.SubscriberEndpoint == client.ToString()).ToList().Any(x => new MessageType(x.TypeName, x.Version) == messageType))
                    continue;
                
                _subscriptions.Save(new Subscription
                                        {
                                            SubscriberEndpoint = client.ToString(),
                                            MessageType = messageType.TypeName + "," + messageType.Version,
                                            Version = messageType.Version.ToString(),
                                            TypeName = messageType.TypeName
                                        });
            }
        }

        public void Unsubscribe(Address client, IEnumerable<MessageType> messageTypes)
        {
            var typeNames = messageTypes.Select(mt => mt.TypeName);
            var subscriptions = _subscriptions.AsQueryable().Where(x => x.TypeName.In(typeNames) && x.SubscriberEndpoint == client.ToString()).ToList();

            foreach (var subscription in subscriptions)
                _subscriptions.Remove(Query.EQ("_id", subscription._id));
        }

        public IEnumerable<Address> GetSubscriberAddressesForMessage(IEnumerable<MessageType> messageTypes)
        {
            var typeNames = messageTypes.Select(mt => mt.TypeName);
            return _subscriptions.AsQueryable()
                    .Where(s => s.TypeName.In(typeNames)).ToList()
                    .Where(s => messageTypes.Contains(new MessageType(s.TypeName, s.Version)))
                    .Select(s => Address.Parse(s.SubscriberEndpoint))
                    .Distinct();
        }

        public void Init()
        {
                var v2XSubscriptions = _subscriptions.AsQueryable().Where(s => s.TypeName == null).ToList();
                if (v2XSubscriptions.Count == 0)
                    return;

                Logger.DebugFormat("Found {0} v2X subscriptions going to upgrade", v2XSubscriptions.Count);

                foreach (var v2XSubscription in v2XSubscriptions)
                {
                    var mt = new MessageType(v2XSubscription.MessageType);
                    v2XSubscription.Version = mt.Version.ToString();
                    v2XSubscription.TypeName = mt.TypeName;

                    _subscriptions.Save(v2XSubscription);

                }
                Logger.InfoFormat("{0} v2X subscriptions upgraded", v2XSubscriptions.Count);
        }

        static readonly ILog Logger = LogManager.GetLogger(typeof(ISubscriptionStorage));
    }
}
