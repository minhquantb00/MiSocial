using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Guard
{
    public static class GuardExtensions
    {
        public static TReturn NotNull<TReturn>(this TReturn value, string message = "")
        {
            if (value == null)
                throw new NullReferenceException(message);
            return value;
        }
    }
}
