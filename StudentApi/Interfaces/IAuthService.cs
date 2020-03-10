using StudentApi.Entities.Authorization;
using StudentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Interfaces
{
    public interface IAuthService
    {
        Task<UserInfo> AuthenticateUser(UserInfo info);

        Task<AdminInfo> AuthenticateAdmin(AdminInfo info);

        Task<string> GetUserPassword(string userId);
        Task<string> GetUserLogin(string userId);
        Task<string> GetUserGitHubLogin(string userId);

    }
}
