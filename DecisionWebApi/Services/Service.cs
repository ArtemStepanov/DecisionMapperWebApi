using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecisionWebApi.DbWorkers;
using DecisionWebApi.Exceptions;
using DecisionWebApi.Helpers;
using DecisionWebApi.Models;
using Newtonsoft.Json.Linq;

namespace DecisionWebApi.Services
{
    public class Service<TModel> where TModel : class, IEntity, new()
    {
        private IDbWorker<TModel> _dbWorker;

        public Service(IDbWorker<TModel> dbWorker)
        {
            _dbWorker = dbWorker;
        }

        public async Task<TModel> Get(int id)
        {
            return await _dbWorker.Get(id);
        }

        public async Task<ICollection<TModel>> GetAll()
        {
            return await _dbWorker.Get();
        }

        public async Task Update(JObject model)
        {
            var id = model.ContainsKey("Id") ? model.Value<int>("Id") : -1;
            var objectDeconstruct = new JObjectDeconstructor<TModel>(model);

            if (id < 0)
            {
                var newEntity = objectDeconstruct.GetObjectInstance();
                if (objectDeconstruct.AllRequiredPropertiesForJObjectOfModelNotEmptyOrNull())
                {
                    await Create(newEntity);
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            else
            {
                var oldEntity = await _dbWorker.Get(id);
                if (oldEntity == null)
                {
                    throw new KeyNotFoundException();
                }

                objectDeconstruct.Patch(oldEntity);
                await _dbWorker.Update(oldEntity);
            }
        }

        public async Task Create(TModel newModel)
        {
            /*
             * Checks if id of new entity greater than zero.
             * If not - increment id to (MaxId from database + 1).
             */
            if (newModel.Id <= 0)
            {
                newModel.Id = await MaxId() + 1;
            }

            if (await _dbWorker.HasEntity(newModel.Id))
            {
                throw new EntityExistException();
            }

            await _dbWorker.Create(newModel);
        }

        public async Task Delete(int id)
        {
            if (!await _dbWorker.HasEntity(id))
            {
                throw new KeyNotFoundException();
            }

            await _dbWorker.Delete(id);
        }

        private async Task<int> MaxId()
        {
            var entities = await _dbWorker.Get();
            var maxId = 0;
            if (entities != null)
            {
                maxId = entities.Count > 0 ? entities.Max(x => x.Id) : maxId;
            }

            return maxId;
        }
    }
}