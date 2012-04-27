using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace NServiceBus.Unicast.Subscriptions.Mongo
{
    public class Subscription
    {
        public ObjectId _id { get; set; }
        public string SubscriberEndpoint { get; set; }
        public string MessageType { get; set; }
        public string Version { get; set; }
        public string TypeName { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Subscription)) return false;
            return Equals((Subscription)obj);
        }

        public bool Equals(Subscription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.SubscriberEndpoint, SubscriberEndpoint) && Equals(other.MessageType, MessageType) && Equals(other.Version, Version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SubscriberEndpoint != null ? SubscriberEndpoint.GetHashCode() : 0) * 397) ^ (MessageType != null ? MessageType.GetHashCode() : 0);
            }
        }
    }
}
