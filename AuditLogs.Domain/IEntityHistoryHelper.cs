using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public interface IEntityHistoryHelper
    {
        List<EntityChangeInfo> CreateChangeList(ICollection<EntityEntry> entityEntries);

        void UpdateChangeList(List<EntityChangeInfo> entityChanges);
    }
}
