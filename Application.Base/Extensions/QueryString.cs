using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Application.Base.Extensions
{
    public class QueryString : NameValueCollection
    {
        public QueryString()
        {
        }

        public QueryString(string queryString)
        {
            FillFromString(queryString);
        }

        public QueryString(NameValueCollection queryString)
            : base(queryString)
        {
        }

        [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
        public static string ExtractQuerystring(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Contains("?"))
                {
                    return s.Substring(s.IndexOf("?") + 1);
                }
            }

            return s;
        }

        public QueryString FillFromString(string s, bool urlDecode = false)
        {
            base.Clear();
            if (string.IsNullOrEmpty(s))
            {
                return this;
            }

            foreach (string keyValuePair in ExtractQuerystring(s).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair))
                {
                    continue;
                }

                string[] split = keyValuePair.Split('=');
                base.Add(split[0], split.Length == 2 ? (urlDecode ? HttpUtility.UrlDecode(split[1]) : split[1]) : "");
            }

            return this;
        }

        public new QueryString Add(string name, string value)
        {
            return Add(name, value, false);
        }

        public QueryString Add(string name, string value, bool isUnique)
        {
            string existingValue = base[name];
            if (string.IsNullOrEmpty(existingValue))
            {
                base.Add(name, HttpUtility.UrlEncode(value));
            }
            else if (isUnique)
            {
                base[name] = HttpUtility.UrlEncode(value);
            }
            else
            {
                base[name] += "," + HttpUtility.UrlEncode(value);
            }

            return this;
        }

        public new QueryString Remove(string name)
        {
            string existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
            {
                base.Remove(name);
            }
            return this;
        }

        public QueryString Reset()
        {
            base.Clear();
            return this;
        }

        public new string this[string name]
        {
            get { return HttpUtility.UrlDecode(base[name]); }
        }

        public new string this[int index]
        {
            get { return HttpUtility.UrlDecode(base[index]); }
        }

        public bool Contains(string name)
        {
            string existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }

    }
}
