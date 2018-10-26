using System.Collections.Generic;
using System.Threading.Tasks;
using DecisionWebApi.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DecisionWebApi.DbWorkers
{
    public class MongoDbWorker<TEntity> : IDbWorker<TEntity> where TEntity : class, IEntity, new()
    {
        private IMongoDatabase _mongoDatabase;
        private readonly string _collectionName;

        public MongoDbWorker(IConfiguration config, string collectionName)
        {
            var connectionString = config.GetConnectionString("Mongo");
            var connection = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(connection.ToMongoUrl());
            _mongoDatabase = mongoClient.GetDatabase(connection.DatabaseName);
            _collectionName = collectionName;
        }

        private IMongoCollection<TEntity> Entities => _mongoDatabase.GetCollection<TEntity>(_collectionName);

        public async Task<ICollection<TEntity>> Get()
        {
            return await Entities.Find(Builders<TEntity>.Filter.Empty).ToListAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            return await Entities.Find(new BsonDocument("_id", id)).FirstOrDefaultAsync();
        }

        public async Task Update(TEntity newEntity)
        {
            await Entities.ReplaceOneAsync(new BsonDocument("_id", newEntity.Id), newEntity);
        }

        public async Task Create(TEntity newEntity)
        {
            await Entities.InsertOneAsync(newEntity);
        }

        public async Task Delete(int id)
        {
            await Entities.DeleteOneAsync(new BsonDocument("_id", id));
        }

        public async Task<bool> HasEntity(int id)
        {
            return await Get(id) != null;
        }
    }
}