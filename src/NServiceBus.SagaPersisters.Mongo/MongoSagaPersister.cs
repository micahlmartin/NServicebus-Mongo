using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NServiceBus.Persistence.Mongo;
using NServiceBus.Saga;
using MongoDB.Driver.Linq;
using System.Linq;
using MongoDB.Driver.Builders;

namespace NServiceBus.SagaPersisters.Mongo
{
    public class MongoSagaPersister : ISagaPersister
    {
        private MongoCollection<BsonDocument> _sagas;
        private MongoCollection<SagaUniqueIdentity> _sagaIds;
        public const string MetadataPropertyName = "__metadata";
 
        public MongoSagaPersister(MongoDatabase database)
        {
            _sagas = database.GetCollection(MongoPersistenceConstants.SagaCollectionName);
            _sagaIds = database.GetCollection<SagaUniqueIdentity>(MongoPersistenceConstants.SagaUniqueIdentityCollectionName);
        }

        public void Save(IContainSagaData saga)
        {
            var uniqueValue = StoreUniqueProperty(saga);
            _sagas.Save(saga);

            if(uniqueValue.HasValue)
                SetUniqueValueMetadata(saga, uniqueValue.Value);
        }

        public void Update(IContainSagaData saga)
        {

            var p = UniqueAttribute.GetUniqueProperty(saga);

            //There's no unique property so just update the saga
            if (!p.HasValue)
            {
                _sagas.Save(saga);
                return;
            }

            var uniqueProperty = p.Value;

            //if the user just added the unique property to a saga with existing data we need to set it
            string storedValue = null;
            BsonElement metadata;
            if (_sagas.FindOne(Query.EQ("_id", saga.Id)).TryGetElement(MetadataPropertyName, out metadata))
                storedValue = metadata.Value.IsString ? metadata.Value.AsString : null;

            if (storedValue != null)
            {
                var currentValue = uniqueProperty.Value.ToString();
                if (currentValue == storedValue)
                {
                    _sagas.Save(saga);
                    return;
                }
                
                DeleteUniqueProperty(saga, new KeyValuePair<string, object>(uniqueProperty.Key, storedValue));
            }

            var uniqueValue = StoreUniqueProperty(saga);
            _sagas.Save(saga);
            if(uniqueValue.HasValue)
                SetUniqueValueMetadata(saga, uniqueValue.Value);
        }

        public T Get<T>(Guid sagaId) where T : IContainSagaData
        {
            return _sagas.FindOneAs<T>(Query.EQ("_id", sagaId));
        }

        public T Get<T>(string property, object value) where T : IContainSagaData
        {
            if (value == null) return default(T);

            return _sagas.FindOneAs<T>(Query.EQ(property, BsonTypeMapper.MapToBsonValue(value)));
        }

        public void Complete(IContainSagaData saga)
        {
            _sagas.Remove(Query.EQ("_id", saga.Id));
            DeleteUniqueProperty(saga);
        }

        private void DeleteUniqueProperty(IContainSagaData saga)
        {
            var uniqueProperty = UniqueAttribute.GetUniqueProperty(saga);

            if (!uniqueProperty.HasValue)
                return;

            var id = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty.Value);
            _sagaIds.FindAndRemove(Query.EQ("_id", id), SortBy.Null);
        }

        private KeyValuePair<string, object>? StoreUniqueProperty(IContainSagaData saga)
        {
            var uniqueProperty = UniqueAttribute.GetUniqueProperty(saga);

            if (!uniqueProperty.HasValue) return null;

            var id = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty.Value);

            if(_sagaIds.AsQueryable().FirstOrDefault(x => x.Id == id) != null)
                throw new InvalidOperationException("A saga with a unique id of " + uniqueProperty.Value + " already exists");

            _sagaIds.Save(new SagaUniqueIdentity { Id = id, SagaId = saga.Id, UniqueValue = uniqueProperty.Value.Value });

            return uniqueProperty.Value;
        }

        private void SetUniqueValueMetadata(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        {
            var result = _sagas.FindAndModify(Query.EQ("_id", saga.Id), SortBy.Null, MongoDB.Driver.Builders.Update.Set(MetadataPropertyName, uniqueProperty.Value.ToString()), true);
            if(result.ModifiedDocument == null)
                throw new InvalidOperationException();
        }

        private void DeleteUniqueProperty(IContainSagaData saga, KeyValuePair<string, object> uniqueProperty)
        {
            var id = SagaUniqueIdentity.FormatId(saga.GetType(), uniqueProperty);
            var result = _sagas.FindAndModify(Query.EQ("_id", saga.Id), SortBy.Null, MongoDB.Driver.Builders.Update.Set(MetadataPropertyName, BsonTypeMapper.MapToBsonValue(null)), true);
            if(result.ModifiedDocument == null)
                throw new InvalidOperationException();
        }
    }

    public class SagaUniqueIdentity
    {
        [BsonId]
        public string Id { get; set; }
        public Guid SagaId { get; set; }
        public object UniqueValue { get; set; }

        public static string FormatId(Type sagaType, KeyValuePair<string, object> uniqueProperty)
        {
            //use MD5 hash to get a 16-byte hash of the string
            var provider = new MD5CryptoServiceProvider();
            var inputBytes = Encoding.Default.GetBytes(uniqueProperty.Value.ToString());
            var hashBytes = provider.ComputeHash(inputBytes);
            //generate a guid from the hash:
            var value = new Guid(hashBytes);

            return string.Format(string.Format("{0}/{1}/{2}", sagaType.FullName, uniqueProperty.Key, value));
        }
    }
}
