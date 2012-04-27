using System;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.Mongo.Tests
{
    public class When_persisting_a_saga_with_the_same_unique_property_as_another_saga : MongoFixture
    {
        [Test]
        public void It_should_enforce_uniqueness()
        {
            var saga1 = new SagaWithUniqueProperty
            {
                Id = Guid.NewGuid(),
                UniqueString = "whatever"
            };

            var saga2 = new SagaWithUniqueProperty
            {
                Id = Guid.NewGuid(),
                UniqueString = "whatever"
            };

            SaveSaga(saga1);
            Assert.Throws<InvalidOperationException>(() => SaveSaga(saga2));
        }
    }
}