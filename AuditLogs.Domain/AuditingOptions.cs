using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogs.Domain
{
    public class AuditingOptions
    {
        public bool HideErrors { get; set; }

        public bool IsEnabled { get; set; }

        public string ApplicationName { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public bool AlwaysLogOnException { get; set; }

        public List<Type> IgnoredTypes { get; }
        public bool IsIgnoreStaticFileRequest { get; set; }
        public List<HttpMethod> IgnoredMethods { get; set; }
        public List<string> IgnoredPaths { get; set; }

        public bool IsEnabledForGetRequests { get; set; }

        public AuditingOptions()
        {
            IsIgnoreStaticFileRequest = true;
            IsEnabledForGetRequests = true;
            IsEnabled = true;
            IsEnabledForAnonymousUsers = true;
            HideErrors = true;
            AlwaysLogOnException = true;
            IgnoredTypes = new List<Type>
            {
                typeof(Stream),
                typeof(Expression)
            };
            IgnoredMethods = new List<HttpMethod>
            {
                HttpMethod.Options,
                HttpMethod.Head
            };
            IgnoredPaths = new List<string> { "/chathub", "/chathub/negotiate", "/connect/introspect", "/api/medias/files/" };
        }
    }
}
