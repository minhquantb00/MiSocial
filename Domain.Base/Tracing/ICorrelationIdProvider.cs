using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Tracing
{
    public interface ICorrelationIdProvider
    {
        string Get();
    }
}
