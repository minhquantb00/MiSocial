using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Features
{
    public interface IHasModificationTime
    {
        DateTime? UpdatedOn { get; set; }
    }
}
