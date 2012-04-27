using System;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.Mongo.Tests
{
    public class When_updating_a_saga_property_that_does_not_have_a_unique_attribute : MongoFixture
    {
        [Test]
        public void It_should_persist_successfully()
        {
            var saga1 = new SagaWithUniqueProperty()
                            {
                                Id = Guid.NewGuid(),
                                UniqueString = "whatever",
                                NonUniqueString = "notUnique"
                            };

            SaveSaga(saga1);

            UpdateSaga<SagaWithUniqueProperty>(saga1.Id, s => s.NonUniqueString = "notUnique2");
        }
    }

    public class When_updating_a_saga_property_on_a_existing_sagainstance_that_just_got_a_unique_attribute_set : MongoFixture
    {
        [Test]
        public void It_should_set_the_attribute_and_allow_the_update()
        {
            var saga1 = new SagaWithUniqueProperty()
                            {
                                Id = Guid.NewGuid(),
                                UniqueString = "whatever",
                                NonUniqueString = "notUnique"
                            };

            SaveSaga(saga1);

            //fake that the attribute was just added by removing the metadata
            var result = Sagas.FindAndModify(Query.EQ("_id", saga1.Id), SortBy.Null,
                                Update.Set(MongoSagaPersister.MetadataPropertyName, BsonTypeMapper.MapToBsonValue(null)), true);

            UpdateSaga<SagaWithUniqueProperty>(saga1.Id, s => s.UniqueString = "whatever2");

            string value = null;
            BsonElement metadata;
            if(Sagas.FindOne(Query.EQ("_id", saga1.Id)).TryGetElement(MongoSagaPersister.MetadataPropertyName, out metadata))
                value=metadata.Value.AsString;

            Assert.AreEqual("whatever2", value);

        }
    }


    public class When_updating_a_saga_without_unique_properties : MongoFixture
    {
        [Test]
        public void It_should_persist_successfully()
        {
            var saga1 = new SagaWithoutUniqueProperties()
                            {
                                Id = Guid.NewGuid(),
                                UniqueString = "whatever",
                                NonUniqueString = "notUnique"
                            };

            SaveSaga(saga1);

            UpdateSaga<SagaWithoutUniqueProperties>(saga1.Id, s =>
                                                                  {
                                                                      s.NonUniqueString = "notUnique2";
                                                                      s.UniqueString = "whatever2";
                                                                  });
        }
    }
}