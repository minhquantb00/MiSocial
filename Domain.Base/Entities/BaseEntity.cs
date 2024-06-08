using Domain.Base.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Entities
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditable
    {
        public TKey Id { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string Creator { get; set; }
        public string CreatorClientId { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Updater { get; set; }
        public string UpdaterClientId { get; set; }

        public object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}
