using Application.Base.Services;
using AuditLogs.Domain.Entities;
using AuditLogs.Domain;
using Domain.Base.Entities;
using Domain.Base.Features;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Base.Extensions;

namespace AuditLogs.Infrastructure
{
    public class EntityHistoryHelper : IEntityHistoryHelper
    {
        public ILogger<EntityHistoryHelper> Logger { get; set; }
        protected AuditingOptions Options { get; }
        protected IAuditingHelper AuditingHelper { get; }

        private readonly IDateTimeService _dateTimeService;

        public EntityHistoryHelper(
            IOptions<AuditingOptions> options,
            IDateTimeService dateTimeService,
            IAuditingHelper auditingHelper)
        {
            _dateTimeService = dateTimeService;
            AuditingHelper = auditingHelper;
            Options = options.Value;

            Logger = NullLogger<EntityHistoryHelper>.Instance;
        }

        public virtual List<EntityChangeInfo> CreateChangeList(ICollection<EntityEntry> entityEntries)
        {
            var list = new List<EntityChangeInfo>();

            foreach (var entry in entityEntries)
            {
                if (!ShouldSaveEntityHistory(entry))
                {
                    continue;
                }

                var entityChange = CreateEntityChangeOrNull(entry);
                if (entityChange == null)
                {
                    continue;
                }

                list.Add(entityChange);
            }

            return list;
        }

        private EntityChangeInfo CreateEntityChangeOrNull(EntityEntry entityEntry)
        {
            var entity = entityEntry.Entity;

            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                case EntityState.Modified:
                    changeType = IsDeleted(entityEntry) ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    return null;
            }

            var entityId = GetEntityId(entity);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                return null;
            }

            var entityType = entity.GetType();
            var entityChange = new EntityChangeInfo
            {
                ChangeType = changeType,
                EntityEntry = entityEntry,
                EntityId = entityId,
                EntityTypeFullName = entityType.FullName,
                PropertyChanges = GetPropertyChanges(entityEntry),
                EntityTenantId = GetTenantId(entity)
            };

            return entityChange;
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private List<EntityPropertyChangeInfo> GetPropertyChanges(EntityEntry entityEntry)
        {
            var propertyChanges = new List<EntityPropertyChangeInfo>();
            var properties = entityEntry.Metadata.GetProperties();
            var isCreated = IsCreated(entityEntry);
            var isDeleted = IsDeleted(entityEntry);

            foreach (var property in properties)
            {
                var propertyEntry = entityEntry.Property(property.Name);
                if (ShouldSavePropertyHistory(propertyEntry, isCreated || isDeleted))
                {
                    propertyChanges.Add(new EntityPropertyChangeInfo
                    {
                        NewValue = isDeleted ? null : JsonConvert.SerializeObject(propertyEntry.CurrentValue),
                        OriginalValue = isCreated ? null : JsonConvert.SerializeObject(propertyEntry.OriginalValue),
                        PropertyName = property.Name,
                        PropertyTypeFullName = property.ClrType.GetFirstGenericArgumentIfNullable().FullName
                    });
                }
            }

            return propertyChanges;
        }

        private bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false)
        {
            if (entityEntry.State == EntityState.Detached ||
                entityEntry.State == EntityState.Unchanged)
            {
                return false;
            }

            var entityType = entityEntry.Metadata.ClrType;

            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                return false;
            }

            if (AuditingHelper.IsEntityHistoryEnabled(entityType))
            {
                return true;
            }

            return defaultValue;
        }

        private bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            if (propertyEntry.Metadata.Name == "Id")
            {
                return false;
            }

            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo != null && propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var entityType = propertyEntry.EntityEntry.Entity.GetType();
            if (entityType.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                if (propertyInfo == null || !propertyInfo.IsDefined(typeof(AuditedAttribute), true))
                {
                    return false;
                }
            }

            var isModified = !(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null);
            if (isModified)
            {
                return true;
            }

            return defaultValue;
        }

        /// <summary>
        /// Updates change time, entity id and foreign keys after SaveChanges is called.
        /// </summary>
        public void UpdateChangeList(List<EntityChangeInfo> entityChanges)
        {
            foreach (var entityChange in entityChanges)
            {
                /* Update change time */

                entityChange.ChangeTime = GetChangeTime(entityChange);

                /* Update entity id */

                var entityEntry = entityChange.EntityEntry as EntityEntry;
                entityChange.EntityId = GetEntityId(entityEntry.Entity);

                /* Update foreign keys */

                var foreignKeys = entityEntry.Metadata.GetForeignKeys();

                foreach (var foreignKey in foreignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var propertyEntry = entityEntry.Property(property.Name);
                        var propertyChange = entityChange.PropertyChanges.FirstOrDefault(pc => pc.PropertyName == property.Name);

                        if (propertyChange == null)
                        {
                            if (!(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null))
                            {
                                // Add foreign key
                                entityChange.PropertyChanges.Add(new EntityPropertyChangeInfo
                                {
                                    NewValue = JsonConvert.SerializeObject(propertyEntry.CurrentValue),
                                    OriginalValue = JsonConvert.SerializeObject(propertyEntry.OriginalValue),
                                    PropertyName = property.Name,
                                    PropertyTypeFullName = property.ClrType.GetFirstGenericArgumentIfNullable().FullName
                                });
                            }

                            continue;
                        }

                        if (propertyChange.OriginalValue == propertyChange.NewValue)
                        {
                            var newValue = JsonConvert.SerializeObject(propertyEntry.CurrentValue);
                            if (newValue == propertyChange.NewValue)
                            {
                                // No change
                                entityChange.PropertyChanges.Remove(propertyChange);
                            }
                            else
                            {
                                // Update foreign key
                                propertyChange.NewValue = newValue;
                            }
                        }
                    }
                }
            }
        }


        protected virtual Guid? GetTenantId(object entity)
        {
            return null; //TODO: Implement MultiTenant
        }

        private DateTime GetChangeTime(EntityChangeInfo entityChange)
        {
            var entity = (entityChange.EntityEntry as EntityEntry).Entity;
            switch (entityChange.ChangeType)
            {
                case EntityChangeType.Created:
                    return (entity as IHasCreationTime)?.CreatedOn ?? _dateTimeService.Now;
                case EntityChangeType.Deleted:
                    return (entity as IHasModificationTime)?.UpdatedOn ?? _dateTimeService.Now;
                case EntityChangeType.Updated:
                    return (entity as IHasModificationTime)?.UpdatedOn ?? _dateTimeService.Now;
                default:
                    throw new Exception($"Unknown {nameof(EntityChangeInfo)}: {entityChange}");
            }
        }

        private string GetEntityId(object entityAsObj)
        {
            if (!(entityAsObj is IEntity entity))
            {
                throw new Exception($"Entities should implement the {typeof(IEntity).AssemblyQualifiedName} interface! Given entity does not implement it: {entityAsObj.GetType().AssemblyQualifiedName}");
            }

            var keys = entity.GetKeys();
            if (keys.All(k => k == null))
            {
                return null;
            }

            return string.Join(',', keys);
        }

        private bool IsCreated(EntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        private bool IsDeleted(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }

            var entity = entityEntry.Entity;
            return entity is ISoftDeletable && (entity as ISoftDeletable).Deleted;
        }
    }
}
