using System.Collections.Generic;
using System.Threading.Tasks;
using DecisionWebApi.Models;

namespace DecisionWebApi.DbWorkers
{
    public interface IDbWorker<TEntity> where TEntity : class, IEntity, new()
    {
        Task<ICollection<TEntity>> Get();
        Task<TEntity> Get(int id);
        Task Update(TEntity newEntity);
        Task Create(TEntity entity);
        Task Delete(int id);
        Task<bool> HasEntity(int id);
    }
}