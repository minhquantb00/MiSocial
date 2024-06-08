using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public interface IAuditingHelper
    {
        bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false);

        bool IsEntityHistoryEnabled(Type entityType, bool defaultValue = false);

        AuditLogInfo CreateAuditLogInfo();

        AuditLogActionInfo CreateAuditLogAction(
            AuditLogInfo auditLog,
            Type type,
            MethodInfo method,
            object[] arguments
        );

        AuditLogActionInfo CreateAuditLogAction(
            AuditLogInfo auditLog,
            Type type,
            MethodInfo method,
            IDictionary<string, object> arguments
        );
        void ExecutePostContributors(AuditLogInfo auditLog);
    }
}
