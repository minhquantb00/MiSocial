using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Tracing
{
    public class CorrelationIdOptions
    {
        public string HttpHeaderName { get; set; } = "X-Correlation-Id";

        public bool SetResponseHeader { get; set; } = true;
    }
}
