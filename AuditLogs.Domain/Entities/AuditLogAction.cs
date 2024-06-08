using Domain.Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public class AuditLogAction : BaseEntity<long>
    {
        public virtual Guid? TenantId { get; set; }

        public virtual long AuditLogId { get; set; }

        public virtual string ServiceName { get; set; }

        public virtual string MethodName { get; set; }

        public virtual string Parameters { get; set; }

        public virtual DateTime ExecutionTime { get; set; }

        public virtual int ExecutionDuration { get; set; }

        [NotMapped]
        public virtual ExtraPropertyDictionary ExtraProperties { get; set; }
    }
}
