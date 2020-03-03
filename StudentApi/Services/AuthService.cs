using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StudentApi.Helpers;
using StudentApi.Interfaces;
using StudentApi.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly SanStudentContext context;

        public AuthService(SanStudentContext ctx)
        {
            context = ctx;
        }                                                                  
        public async Task<UserInfo> AuthenticateUser(UserInfo info)
        {
            string cryptedPass = Cryptor.Encrypt(info.Password);
            return await context.Users.Where(u => u.AlbumNumber == info.AlbumNumber && u.Password == cryptedPass).FirstOrDefaultAsync();
        }
    }
}
