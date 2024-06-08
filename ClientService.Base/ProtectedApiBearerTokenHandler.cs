using Domain.Base.Tracing;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientService.Base
{
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IIdentityServerClient _identityServerClient;
        private readonly CorrelationIdOptions _options;

        public ProtectedApiBearerTokenHandler(
            IIdentityServerClient identityServerClient,
            CorrelationIdOptions options)
        {
            _identityServerClient = identityServerClient
                ?? throw new ArgumentNullException(nameof(identityServerClient));
            _options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IEnumerable<string> correlationId = new List<string>();
            request.Headers.TryGetValues(_options.HttpHeaderName, out correlationId);

            var accessToken = await _identityServerClient.RequestClientCredentialsTokenAsync(string.Join(',', correlationId));

            request.SetBearerToken(accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
