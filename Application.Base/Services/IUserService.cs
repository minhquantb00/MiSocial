using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Services
{
    public interface IUserService
    {
        string GetCurrentUserName();
        string GetCurrentUserId();
        string GetCurrentClientId();
        bool HasRole(string role);
        bool HasPermission(string permission);
    }
}
