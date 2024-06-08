using Application.Base.Services;
using AuditLogs.Domain;
using Domain.Base.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : AppDbContext<TContext>
    {
        private readonly TContext _context;
        private Dictionary<string, dynamic> _repositories;
        private readonly IServiceProvider _serviceProvider;
        private IAuditingManager _auditingManager => _serviceProvider.GetRequiredService<IAuditingManager>();
        private IEntityHistoryHelper _entityHistoryHelper => _serviceProvider.GetRequiredService<IEntityHistoryHelper>();
        private ILogger<UnitOfWork<TContext>> _logger => _serviceProvider.GetRequiredService<ILogger<UnitOfWork<TContext>>>();
        private IDateTimeService _dateTimeService => _serviceProvider.GetRequiredService<IDateTimeService>();
        private IUserService _userService => _serviceProvider.GetRequiredService<IUserService>();

        public UnitOfWork(TContext context,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }
        IRepository<TEntity, TKey> IUnitOfWork.Repository<TEntity, TKey>()
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();
            var type = typeof(TEntity).Name;
            if (_repositories.ContainsKey(type))
                return (IRepository<TEntity, TKey>)_repositories[type];
            var repositoryType = typeof(EfRepository<TEntity, TKey>);
            _repositories.Add(type, Activator.CreateInstance(
                repositoryType, _context, _dateTimeService, _userService)
            );
            return _repositories[type];
        }
        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public void Rollback()
        {
            _context.Database.RollbackTransaction();
        }
        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public int SaveChanges()
        {
            BeforeSaveChange();
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            BeforeSaveChange();
            var res = await _context.SaveChangesAsync();
            AfterSaveChange();
            return res;
        }
        public void CommitTransaction()
        {
            _context.Database.CommitTransaction();
        }
        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }
        private void BeforeSaveChange()
        {
            try
            {
                var auditLog = _auditingManager?.Current?.Log;

                List<EntityChangeInfo> entityChangeList = null;
                if (auditLog != null)
                {
                    var entryEntities = _context.ChangeTracker.Entries().ToList();
                    entityChangeList = _entityHistoryHelper.CreateChangeList(entryEntities);
                    _entityHistoryHelper.UpdateChangeList(entityChangeList);
                    auditLog.EntityChanges.AddRange(entityChangeList);
                    _logger.LogDebug($"Added {entityChangeList.Count} entity changes to the current audit log");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
        private void AfterSaveChange()
        {
            try
            {
                foreach (var entity in _context.ChangeTracker.Entries())
                {
                    entity.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
