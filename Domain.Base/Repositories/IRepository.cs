using Domain.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Repositories
{
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification = null);
        Task<int> CountAsync(ISpecification<TEntity> specification = null);
        IEnumerable<TEntity> Get(ISpecification<TEntity> specification = null);
        IQueryable<TEntity> Query { get; }
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        IEnumerable<TEntity> GetByIds(ICollection<TKey> ids);
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        void SoftDelete(TEntity entity);
        void SoftDeleteRange(IEnumerable<TEntity> entities);

        IQueryable<TEntity> GetFromSqlAsync(string sql, params object[] parameters);
    }
}
