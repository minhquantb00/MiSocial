using Application.Base.Services;
using Domain.Base.Entities;
using Domain.Base.Features;
using Domain.Base.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base
{
    public partial class EfRepository<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey>
    {
        private readonly DbContext _context;
        private DbSet<T> _entities;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUserService _userService;

        public EfRepository(DbContext context,
            IDateTimeService dateTimeService,
            IUserService userService)
        {
            _context = context;
            _dateTimeService = dateTimeService;
            _userService = userService;
        }

        #region interface members

        public async Task<IEnumerable<T>> GetAsync(ISpecification<T> specification = null)
        {
            return await ApplySpecification(specification)
                .ToListAsync();
        }
        public async Task<int> CountAsync(ISpecification<T> specification = null)
        {
            return await ApplySpecification(specification).CountAsync();
        }
        public IEnumerable<T> Get(ISpecification<T> specification = null)
        {
            return ApplySpecification(specification);
        }
        public IQueryable<T> Query
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }
        public virtual T GetById(TKey id)
        {
            return Entities.FirstOrDefault(x => x.Id.Equals(id));
        }

        public IEnumerable<T> GetByIds(ICollection<TKey> ids)
        {
            return Entities.Where(x => ids.Contains(x.Id));
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            return await Entities.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
        public virtual void Add(T entity)
        {
            AddCreationAuditInfo(entity);
            Entities.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    AddCreationAuditInfo(entity);
                }
                // insert all in one step
                Entities.AddRange(entities);
            }
        }

        public async Task AddAsync(T entity)
        {
            AddCreationAuditInfo(entity);
            await Entities.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    AddCreationAuditInfo(entity);
                }
                await Entities.AddRangeAsync(entities);
            }
        }

        public virtual void Update(T entity)
        {
            ChangeStateToModifiedIfApplicable(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                ChangeStateToModifiedIfApplicable(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }

        #endregion

        #region Helpers
        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }

                return _entities;
            }
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T, TKey>.GetQuery(Entities.AsNoTracking(), spec);
        }

        private void ChangeStateToModifiedIfApplicable(T entity)
        {
            AddUpdatingAuditInfo(entity);
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Modified;
            }
        }
        private void AddCreationAuditInfo(T entity)
        {
            if (entity is IAuditable)
            {
                var auditEntity = (entity as IAuditable);
                if (!auditEntity.CreatedOn.HasValue)
                    auditEntity.CreatedOn = _dateTimeService.Now;
                if (!auditEntity.UpdatedOn.HasValue)
                    auditEntity.UpdatedOn = (entity as IAuditable).CreatedOn;
                if (!auditEntity.Creator.HasValue())
                    (entity as IAuditable).Creator = _userService.GetCurrentUserId();
                if (!auditEntity.CreatorClientId.HasValue())
                    (entity as IAuditable).CreatorClientId = _userService.GetCurrentClientId();
            }
        }
        private void AddUpdatingAuditInfo(T entity)
        {
            if (entity is IAuditable)
            {
                var auditEntity = (entity as IAuditable);
                auditEntity.UpdatedOn = _dateTimeService.Now;
                auditEntity.Updater = _userService.GetCurrentUserId();
                auditEntity.UpdaterClientId = _userService.GetCurrentClientId();
            }
        }

        public IQueryable<T> GetFromSqlAsync(string sql, params object[] parameters)
        {
            return Entities.FromSqlRaw(sql, parameters).AsNoTracking();
        }

        public void SoftDelete(T entity)
        {
            if (entity is ISoftDeletable)
            {
                var auditEntity = (entity as ISoftDeletable);
                auditEntity.Deleted = true;
                ChangeStateToModifiedIfApplicable(entity);
            }
        }

        public void SoftDeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable)
                {
                    var auditEntity = (entity as ISoftDeletable);
                    auditEntity.Deleted = true;
                    ChangeStateToModifiedIfApplicable(entity);
                }
            }
        }
        #endregion

    }
}
