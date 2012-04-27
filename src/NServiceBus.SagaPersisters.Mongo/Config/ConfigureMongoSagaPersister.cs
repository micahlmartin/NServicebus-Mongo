using MongoDB.Driver;
using NServiceBus.Persistence.Mongo.Config;
using NServiceBus.SagaPersisters.Mongo;

namespace NServiceBus
{
    public static class ConfigureMongoSagaPersister
    {
        public static Configure MongoSagaPersister(this Configure config)
        {
            if (!config.Configurer.HasComponent<MongoDatabase>())
                config.MongoPersistence();

            config.Configurer.ConfigureComponent<MongoSagaPersister>(DependencyLifecycle.InstancePerCall);

            return config;
        }
    }
}
