using Domain.Base.Tracing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base.Tracing
{
    public class CorrelationIdMiddleware : IMiddleware
    {
        private readonly CorrelationIdOptions _options;
        private readonly ICorrelationIdProvider _correlationIdProvider;

        public CorrelationIdMiddleware(IOptions<CorrelationIdOptions> options,
            ICorrelationIdProvider correlationIdProvider)
        {
            _options = options.Value;
            _correlationIdProvider = correlationIdProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = _correlationIdProvider.Get();

            try
            {
                await next(context);
            }
            finally
            {
                CheckAndSetCorrelationIdOnResponse(context, _options, correlationId);
            }
        }

        protected virtual void CheckAndSetCorrelationIdOnResponse(
            HttpContext httpContext,
            CorrelationIdOptions options,
            string correlationId)
        {
            if (httpContext.Response.HasStarted)
            {
                return;
            }

            if (!options.SetResponseHeader)
            {
                return;
            }

            if (!httpContext.Response.Headers.ContainsKey(options.HttpHeaderName))
            {
                httpContext.Response.Headers[options.HttpHeaderName] = correlationId;
                httpContext.Response.Cookies.Append(options.HttpHeaderName, correlationId);
            }
        }
    }
}
