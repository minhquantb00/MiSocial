using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientService.Base
{
    public interface IIdentityServerClient
    {
        Task<string> RequestClientCredentialsTokenAsync(string correlationId = null);
    }
}
