using System;
using DecisionWebApi.DbWorkers;
using DecisionWebApi.Enumerations;
using DecisionWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DecisionWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbWorker<TEntity> (this IServiceCollection services, IConfiguration config, string collectionName)
            where TEntity: class, IEntity, new()
        {
            IDbWorker<TEntity> dbWorker = null;
            var appDatabaseString = config.GetValue<string>("ApplicationDatabase");

            if (Enum.TryParse<ApplicationDatabaseType>(appDatabaseString, out ApplicationDatabaseType dbType))
            {
                switch (dbType)
                {
                    case ApplicationDatabaseType.Mongo:
                        dbWorker = new MongoDbWorker<TEntity>(config, collectionName);
                        break;
                    case ApplicationDatabaseType.Postgres:
                        {
                            var connectionString = config.GetConnectionString(dbType.ToString());
                            var options = new DbContextOptionsBuilder<PostgreDbWorker<TEntity>>();

                            dbWorker = new PostgreDbWorker<TEntity>(options.UseNpgsql(connectionString).Options, collectionName);
                            break;
                        }
                }

                services.TryAddSingleton<IDbWorker<TEntity>>(dbWorker);
            }

            return services;
        }
    }
}
