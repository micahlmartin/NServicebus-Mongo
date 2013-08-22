﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using NServiceBus.Persistence.Mongo;

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

            var client = new MongoClient(MongoPersistenceConstants.DefaultUrl);
            var db = client.GetServer().GetDatabase(DatabaseNamingConvention());

            return MongoPersistence(config, client.GetServer(), db);
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
            var client = new MongoClient(connectionStringValue);

            var mongoUrl = MongoUrl.Create(connectionStringValue);
            MongoDatabase db;
            
            if (!string.IsNullOrEmpty(database))
                db = client.GetServer().GetDatabase(database);
            else if (!string.IsNullOrEmpty(mongoUrl.DatabaseName))
                db = client.GetServer().GetDatabase(mongoUrl.DatabaseName);
            else
                db = client.GetServer().GetDatabase(DatabaseNamingConvention());

            return MongoPersistence(config, client.GetServer(), db);
        }

        static Configure MongoPersistence(this Configure config, MongoServer server, MongoDatabase database)
        {
            if(config == null) throw new ArgumentNullException("config");
            if(server == null) throw new ArgumentNullException("server");
            if(database == null) throw new ArgumentNullException("database");

            config.Configurer.RegisterSingleton<MongoDatabase>(database);
            config.Configurer.RegisterSingleton<MongoServer>(server);

            return config;
        }

        static Func<string> DatabaseNamingConvention = () => Configure.EndpointName;
    }
}
