using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecisionWebApi.DbWorkers;
using DecisionWebApi.Exceptions;
using DecisionWebApi.Helpers;
using DecisionWebApi.Models;

namespace DecisionWebApi.Test.DbWorkers
{
    public class ListDbWorker<T> : IDbWorker<T> where T : class, IEntity, new()
    {
        private readonly ICollection<T> _entitiesDatabase;

        public ListDbWorker()
        {
            _entitiesDatabase = new List<T>();
        }

        public Task<ICollection<T>> Get()
        {
            return Task.FromResult(_entitiesDatabase);
        }

        public Task<T> Get(int id)
        {
            return Task.FromResult(_entitiesDatabase.FirstOrDefault(car => car.Id == id));
        }

        public async Task Update(T newEntity)
        {
            var oldEntity = await Get(newEntity.Id);
            _entitiesDatabase.Remove(oldEntity);
            _entitiesDatabase.Add(newEntity);
        }

        public async Task Create(T entity)
        {
            if (!(await HasEntity(entity.Id)))
            {
                _entitiesDatabase.Add(entity);
            }
            else
            {
                throw new EntityExistException();
            }
        }

        public async Task Delete(int id)
        {
            var toDelete = await Get(id);
            if (toDelete == null)
            {
                throw new KeyNotFoundException();
            }

            _entitiesDatabase.Remove(toDelete);
        }

        public Task<bool> HasEntity(int id)
        {
            return Task.FromResult(_entitiesDatabase.Any(entity => entity.Id == id));
        }
    }
}