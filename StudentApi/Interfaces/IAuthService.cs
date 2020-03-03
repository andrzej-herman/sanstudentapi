using StudentApi.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Interfaces
{
    public interface IAuthService
    {
        Task<UserInfo> AuthenticateUser(UserInfo info);

        Task<string> GetUserPassword(string userId);
    }
}
