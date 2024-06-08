using AuditLogs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public class AuditLogActionInfo
    {
        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public string Parameters { get; set; }

        public DateTime ExecutionTime { get; set; }

        public int ExecutionDuration { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; }

        public AuditLogActionInfo()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}
