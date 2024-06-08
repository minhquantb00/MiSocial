using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain.Entities
{
    public class ExtraPropertyDictionary : Dictionary<string, object>
    {
        public ExtraPropertyDictionary()
        {

        }

        public ExtraPropertyDictionary(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }
    }
}
