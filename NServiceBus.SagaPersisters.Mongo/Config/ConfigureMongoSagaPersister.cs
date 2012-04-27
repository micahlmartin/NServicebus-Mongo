using MongoDB.Driver;
using NServiceBus.SagaPersisters.Mongo;
using NServiceBus.SagaPersisters.Mongo.Config;

namespace NServiceBus
{
    public static class ConfigureMongoSagaPersister
    {
        public static Configure MongoSagaPersister(this Configure config)
        {
            if (!config.Configurer.HasComponent<MongoDatabase>())
                config.MongoPersistence();

            MongoMappingConfiguration.ConfigureMapping();

            config.Configurer.ConfigureComponent<MongoSagaPersister>(DependencyLifecycle.InstancePerCall);

            return config;
        }
    }
}
