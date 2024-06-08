using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientService.Base
{
    public abstract class ClientServiceBase
    {
        protected readonly IIdentityServerClient _identityServerClient;
        protected readonly HttpClient _httpClient;
        public ClientServiceBase(HttpClient httpClient,
        IIdentityServerClient identityServerClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _identityServerClient = identityServerClient ?? throw new ArgumentNullException(nameof(identityServerClient));
        }
    }
}
