﻿using System;
using NUnit.Framework;

namespace NServiceBus.SagaPersisters.Mongo.Tests
{
    public class When_persisting_a_saga_with_the_same_unique_property_as_a_completed_saga : MongoFixture
    {
        [Test]
        public void It_should_persist_successfully()
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
            CompleteSaga<SagaWithUniqueProperty>(saga1.Id);
            SaveSaga(saga2);
        }
    }
}