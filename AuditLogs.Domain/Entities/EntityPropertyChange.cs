using Domain.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public class EntityPropertyChange : BaseEntity<long>
    {
        public virtual Guid? TenantId { get; set; }

        public virtual long EntityChangeId { get; set; }

        public virtual string NewValue { get; set; }

        public virtual string OriginalValue { get; set; }

        public virtual string PropertyName { get; set; }

        public virtual string PropertyTypeFullName { get; set; }
    }
}
