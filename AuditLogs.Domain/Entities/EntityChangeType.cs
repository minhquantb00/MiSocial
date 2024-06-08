using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public enum EntityChangeType
    {
        Created = 0,

        Updated = 1,

        Deleted = 2
    }
}
