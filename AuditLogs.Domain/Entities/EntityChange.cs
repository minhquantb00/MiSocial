using Domain.Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public class EntityChange : BaseEntity<long>
    {
        public virtual long AuditLogId { get; set; }

        public virtual Guid? TenantId { get; set; }

        public virtual DateTime ChangeTime { get; set; }

        public virtual EntityChangeType ChangeType { get; set; }

        public virtual Guid? EntityTenantId { get; set; }

        public virtual string EntityId { get; set; }

        public virtual string EntityTypeFullName { get; set; }

        public virtual ICollection<EntityPropertyChange> PropertyChanges { get; set; }

        [NotMapped]
        public virtual ExtraPropertyDictionary ExtraProperties { get; set; }
    }
}
