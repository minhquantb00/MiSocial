using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class EnumerableExtensions
    {

        #region NameValueCollection

        public static void AddRange(this NameValueCollection initial, NameValueCollection other)
        {
            if (initial == null)
                throw new ArgumentNullException(nameof(initial));

            if (other == null)
                return;

            foreach (var item in other.AllKeys)
            {
                initial.Add(item, other[item]);
            }
        }
        #endregion

        public static void AddRange<T>(this IEnumerable<T> initial, IEnumerable<T> other)
        {
            if (other == null)
                return;

            if (initial is List<T> list)
            {
                list.AddRange(other);
                return;
            }

            initial.Concat(other);
        }
    }
}
