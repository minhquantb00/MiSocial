﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public interface IAuditLogScope
    {
        AuditLogInfo Log { get; }
    }
}