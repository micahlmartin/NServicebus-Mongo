using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus.Saga;

namespace NServiceBus.SagaPersisters.Mongo.Config
{
    public static class MongoMappingConfiguration
    {
        public static void ConfigureMapping()
        {
            var conventionsProfile = new ConventionProfile();
            conventionsProfile.SetIgnoreExtraElementsConvention(new IgnoreAllExtraElementsConvention());
            conventionsProfile.SetIdMemberConvention(new NamedIdMemberConvention("Id"));
            BsonClassMap.RegisterConventions(conventionsProfile, t => true);
        }

        public class IgnoreAllExtraElementsConvention : IIgnoreExtraElementsConvention
        {
            public bool IgnoreExtraElements(Type type)
            {
                return true;
            }
        }
    }
}
