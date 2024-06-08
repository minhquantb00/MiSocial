using Domain.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public class AuditLog : BaseEntity<long>
    {
        public virtual string ApplicationName { get; set; }

        public virtual Guid? UserId { get; set; }

        public virtual string UserName { get; set; }

        public virtual Guid? TenantId { get; set; }

        public virtual string TenantName { get; set; }

        public virtual Guid? ImpersonatorUserId { get; set; }

        public virtual Guid? ImpersonatorTenantId { get; set; }

        public virtual DateTime ExecutionTime { get; set; }

        public virtual int ExecutionDuration { get; set; }

        public virtual string ClientIpAddress { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string ClientId { get; set; }

        public virtual string CorrelationId { get; set; }

        public virtual string BrowserInfo { get; set; }

        public virtual string HttpMethod { get; set; }

        public virtual string Url { get; set; }

        public virtual string Exceptions { get; set; }

        public virtual string Comments { get; set; }

        public virtual int? HttpStatusCode { get; set; }

        public virtual ICollection<EntityChange> EntityChanges { get; set; }

        public virtual ICollection<AuditLogAction> Actions { get; set; }
    }
}
