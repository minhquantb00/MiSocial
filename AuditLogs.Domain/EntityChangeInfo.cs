using AuditLogs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public class EntityChangeInfo
    {
        public DateTime ChangeTime { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public Guid? EntityTenantId { get; set; }

        public string EntityId { get; set; }

        public string EntityTypeFullName { get; set; }

        public List<EntityPropertyChangeInfo> PropertyChanges { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; }

        public virtual object EntityEntry { get; set; } //TODO: Try to remove since it breaks serializability

        public EntityChangeInfo()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }

        public virtual void Merge(EntityChangeInfo changeInfo)
        {
            foreach (var propertyChange in changeInfo.PropertyChanges)
            {
                var existingChange = PropertyChanges.FirstOrDefault(p => p.PropertyName == propertyChange.PropertyName);
                if (existingChange == null)
                {
                    PropertyChanges.Add(propertyChange);
                }
                else
                {
                    existingChange.NewValue = propertyChange.NewValue;
                }
            }

            foreach (var extraProperty in changeInfo.ExtraProperties)
            {
                var key = extraProperty.Key;
                if (ExtraProperties.ContainsKey(key))
                {
                    key = AddCounter(key);
                }

                ExtraProperties[key] = extraProperty.Value;
            }
        }
        private string AddCounter(string str)
        {
            if (str.Contains("__"))
            {
                var splitted = str.Split("__");
                if (splitted.Length == 2)
                {
                    if (int.TryParse(splitted[1], out var num))
                    {
                        return splitted[0] + "__" + (++num);
                    }
                }
            }

            return str + "__2";
        }
    }
}
