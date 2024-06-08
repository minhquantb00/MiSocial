using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Features
{
    public interface IAuditable : IHasModificationTime, IHasCreationTime
    {

        string Updater { get; set; }
        public string UpdaterClientId { get; set; }
        public string CreatorClientId { get; set; }
    }
}
