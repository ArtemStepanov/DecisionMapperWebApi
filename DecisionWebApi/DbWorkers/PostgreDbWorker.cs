using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecisionWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DecisionWebApi.DbWorkers
{
    public class PostgreDbWorker<TEntity> : DbContext, IDbWorker<TEntity> where TEntity : class, IEntity, new()
    {
        private readonly string _tableName;

        public PostgreDbWorker(DbContextOptions options, string tableName) : base(options)
        {
            _tableName = tableName.ToLower();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TEntity>().ToTable(_tableName);
        }

        public DbSet<TEntity> Entities { get; set; }

        public async Task<ICollection<TEntity>> Get()
        {
            return await Entities.ToListAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            return await Entities.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public async Task Update(TEntity newEntity)
        {
            var oldEntity = Entities.First(x => x.Id == newEntity.Id);
            Entry(oldEntity).CurrentValues.SetValues(newEntity);
            await SaveChangesAsync();
        }

        public async Task Create(TEntity entity)
        {
            await Entities.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var foundEntity = await Get(id);
            Entities.Remove(foundEntity);
            await SaveChangesAsync();
        }

        public async Task<bool> HasEntity(int id)
        {
            return await Entities.AnyAsync(x => x.Id == id);
        }
    }
}