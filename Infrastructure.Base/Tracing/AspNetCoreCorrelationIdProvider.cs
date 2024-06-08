using Application.Base.Extensions;
using Domain.Base.Tracing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base.Tracing
{
    public class AspNetCoreCorrelationIdProvider : ICorrelationIdProvider
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected CorrelationIdOptions Options { get; }
        private readonly ILogger<AspNetCoreCorrelationIdProvider> _logger;

        public AspNetCoreCorrelationIdProvider(
            ILogger<AspNetCoreCorrelationIdProvider> logger,
            IHttpContextAccessor httpContextAccessor,
            IOptions<CorrelationIdOptions> options)
        {
            HttpContextAccessor = httpContextAccessor;
            Options = options.Value;
            _logger = logger;
        }

        public virtual string Get()
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers == null)
            {
                return CreateNewCorrelationId();
            }

            string correlationId = HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName];
            if (correlationId.IsNullOrEmpty())
            {
                correlationId = HttpContextAccessor.HttpContext.Request.Cookies[Options.HttpHeaderName];
            }
            if (correlationId.IsNullOrEmpty())
            {
                lock (HttpContextAccessor.HttpContext.Request.Headers)
                {
                    if (correlationId.IsNullOrEmpty())
                    {
                        correlationId = CreateNewCorrelationId();
                        HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName] = correlationId;
                        lock (HttpContextAccessor.HttpContext.Response.Headers)
                        {
                            HttpContextAccessor.HttpContext.Response.Headers[Options.HttpHeaderName] = correlationId;
                            HttpContextAccessor.HttpContext.Response.Cookies.Append(Options.HttpHeaderName, correlationId, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });
                        }
                        _logger.LogInformation($"Add correlation {correlationId}");
                    }
                }
            }

            return correlationId;
        }

        protected virtual string CreateNewCorrelationId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
