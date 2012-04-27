using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus.Saga;

namespace NServiceBus.Persistence.Mongo.Config
{
    public static class MongoMappingConfiguration
    {
        public static void ConfigureMapping()
        {
            var conventionsProfile = new ConventionProfile();
            conventionsProfile.SetIgnoreExtraElementsConvention(new AlwaysIgnoreExtraElementsConvention());
            conventionsProfile.SetIdMemberConvention(new NamedIdMemberConvention("Id"));
            BsonClassMap.RegisterConventions(conventionsProfile, t => typeof(ISagaEntity).IsAssignableFrom(t));
        }
    }
}
