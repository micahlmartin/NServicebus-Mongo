using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using NServiceBus.Persistence.Mongo;
using NServiceBus.Persistence.Mongo.Config;

namespace NServiceBus
{
    public static class ConfigureMongoPersistence
    {
        public static Configure MongoPersistence(this Configure config)
        {
            var connectionStringEntry = ConfigurationManager.ConnectionStrings["NServicebus.Persistence"];

            //user existing config if we can find one
            if (connectionStringEntry != null)
                return MongoPersistenceWithConectionString(config, connectionStringEntry.ConnectionString, null);


            var server = MongoServer.Create(MongoPersistenceConstants.DefaultUrl);
            var db = server.GetDatabase(DatabaseNamingConvention());

            return MongoPersistence(config, server, db);
        }

        public static Configure MongoPersistence(this Configure config, string connectionStringName)
        {
            var connectionStringEntry = GetMongoConnectionString(connectionStringName);
            return MongoPersistenceWithConectionString(config, connectionStringEntry, null);
        }

        public static Configure MongoPersistence(this Configure config, string connectionStringName, string database)
        {
            var connectionString = GetMongoConnectionString(connectionStringName);
            return MongoPersistenceWithConectionString(config, connectionString, database);
        }

        public static Configure MongoPersistence(this Configure config, Func<string> getConnectionString)
        {
            var connectionString = GetMongoConnectionString(getConnectionString);
            return MongoPersistenceWithConectionString(config, connectionString, null);
        }

        public static Configure MongoPersistence(this Configure config, Func<string> getConnectionString, string database)
        {
            var connectionString = GetMongoConnectionString(getConnectionString);
            return MongoPersistenceWithConectionString(config, connectionString, database);
        }

        static string GetMongoConnectionString(Func<string> getConnectionString)
        {
            var connectionString = getConnectionString();

            if (connectionString == null)
                throw new ConfigurationErrorsException("Cannot configure Mongo Persister. No connection string was found");

            return connectionString;
        }

        static string GetMongoConnectionString(string connectionStringName)
        {
            var connectionStringEntry = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringEntry == null)
                throw new ConfigurationErrorsException(string.Format("Cannot configure Mongo Persister. No connection string named {0} was found", connectionStringName));

            return connectionStringEntry.ConnectionString;
        }

        static Configure MongoPersistenceWithConectionString(Configure config, string connectionStringValue, string database)
        {
            var server = MongoServer.Create(connectionStringValue);

            var mongoUrl = MongoUrl.Create(connectionStringValue);
            MongoDatabase db;
            
            if (!string.IsNullOrEmpty(database))
                db = server.GetDatabase(database);
            else if (!string.IsNullOrEmpty(mongoUrl.DatabaseName))
                db = server.GetDatabase(mongoUrl.DatabaseName);
            else
                db = server.GetDatabase(DatabaseNamingConvention());

            return MongoPersistence(config, server, db);
        }

        static Configure MongoPersistence(this Configure config, MongoServer server, MongoDatabase database)
        {
            if(config == null) throw new ArgumentNullException("config");
            if(server == null) throw new ArgumentNullException("server");
            if(database == null) throw new ArgumentNullException("database");

            config.Configurer.RegisterSingleton<MongoDatabase>(database);
            config.Configurer.RegisterSingleton<MongoServer>(server);
            MongoMappingConfiguration.ConfigureMapping();

            return config;
        }

        static Func<string> DatabaseNamingConvention = () => Configure.EndpointName;
    }
}
