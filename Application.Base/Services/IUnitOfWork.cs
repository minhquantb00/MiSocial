using Domain.Base.Entities;
using Domain.Base.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Services
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        Task BeginTransactionAsync();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Rollback();
        Task RollbackTransactionAsync();
        void CommitTransaction();
        Task CommitTransactionAsync();
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey>;
    }
}
