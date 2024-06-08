using Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Infrastructure
{
    public class AuditLogUnitOfWork : UnitOfWork<AuditLogContext>
    {
        public AuditLogUnitOfWork(AuditLogContext context,
            IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
