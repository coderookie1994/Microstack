using Microsoft.Extensions.DependencyInjection;
using Microstack.Repository.Abstractions;
using Microstack.Repository.Providers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microstack.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection collection, Action<ConnectionProviderSettings> settings)
        {
            _ = settings ?? throw new ArgumentNullException($"{nameof(ConnectionProviderSettings)} cannot be empty");
            var connectionProviderSettings = new ConnectionProviderSettings();
            settings(connectionProviderSettings);

            collection.AddSingleton<IMongoClient>(ctx =>
            {
                var connString = connectionProviderSettings.EnvironmentConnectionString 
                ?? connectionProviderSettings.ConfigConnectionString 
                ?? string.Empty;
                if (string.IsNullOrWhiteSpace(connString))
                {
                    throw new ArgumentNullException("No MongoDb connection string found, " +
                        "set environment variable ConnectionString with correct value or provide a suitable value bound to IConfiguration");
                }
                return new MongoClient(connString);
            });
            collection.AddTransient<IPersistenceProvider, MongoProvider>();

            return collection;
        }
    }

    public class ConnectionProviderSettings
    {
        internal string ConfigConnectionString { get; private set; }
        internal string EnvironmentConnectionString { get; private set; }
        public void FromConfiguration(string value)
        {
            ConfigConnectionString = value;
        }

        public void FromEnvironment(string key)
        {
            EnvironmentConnectionString = Environment.GetEnvironmentVariable(key);
        }
    }
}
