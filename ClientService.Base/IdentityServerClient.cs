using Domain.Base.Tracing;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientService.Base
{
    public class IdentityServerClient : IIdentityServerClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientCredentialsTokenRequest _tokenRequest;
        private readonly ILogger<IdentityServerClient> _logger;
        private readonly CorrelationIdOptions _options;

        public IdentityServerClient(
            HttpClient httpClient,
            ClientCredentialsTokenRequest tokenRequest,
            ILogger<IdentityServerClient> logger,
            CorrelationIdOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options;
        }

        public async Task<string> RequestClientCredentialsTokenAsync(string correlationId = null)
        {
            if (!string.IsNullOrEmpty(correlationId))
            {
                if (_httpClient.DefaultRequestHeaders.Contains(_options.HttpHeaderName))
                    _httpClient.DefaultRequestHeaders.Remove(_options.HttpHeaderName);
                _httpClient.DefaultRequestHeaders.Add(_options.HttpHeaderName, correlationId);
            }
            // discover endpoints from metadata
            var disco = await _httpClient.GetDiscoveryDocumentAsync(_httpClient.BaseAddress.ToString());
            if (disco.IsError)
            {
                _logger.LogError(disco.Error);
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }
            _tokenRequest.Address = disco.TokenEndpoint;
            // request the access token token
            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenResponse.IsError)
            {
                _logger.LogError(tokenResponse.Error);
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }
            return tokenResponse.AccessToken;
        }
    }
}
