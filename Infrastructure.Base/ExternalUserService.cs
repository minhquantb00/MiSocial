using Application.Base.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Base
{
    public sealed class ExternalUserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExternalUserService(
            IHttpContextAccessor httpContextAccessor) =>
            this._httpContextAccessor = httpContextAccessor;

        private List<string> getRoles()
        {
            return _httpContextAccessor.HttpContext?.User?.FindAll("role")?.Select(x => x.Value).ToList();
        }
        private List<string> getPermissions()
        {
            return _httpContextAccessor.HttpContext?.User?.FindAll("permission")?.Select(x => x.Value).ToList();
        }

        /// <inheritdoc />
        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("preferred_username")?.Value;
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        }

        public string GetCurrentClientId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("client_id")?.Value;
        }

        public bool HasRole(string role)
        {
            return getRoles().Contains(role);
        }

        public bool HasPermission(string permission)
        {
            return getPermissions().Contains(permission);
        }
    }
}
