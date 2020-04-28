using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StudentApi.Helpers;
using StudentApi.Interfaces;
using StudentApi.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudentApi.Models;
using StudentApi.SendGrid;
using StudentApi.Entities.Data;

namespace StudentApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly SanStudentContext context;

        public AuthService(SanStudentContext ctx)
        {
            context = ctx;
        }

        public async Task<AdminInfo> AuthenticateAdmin(AdminInfo info)
        {
            string cryptedPass = Cryptor.Encrypt(info.Password);
            var checkUser = await context.Users.Where(u => u.AlbumNumber == info.Username && u.Role == "Admin").FirstOrDefaultAsync();
            if (checkUser == null)
            {
                return new AdminInfo
                {
                    LoginResult = false,
                    ErrorUsername = "Użytkownik nie istnieje w systemie",
                    ErrorPassword = null,
                };
            }

            var user = await context.Users.Where(u => u.AlbumNumber == info.Username && u.Password == cryptedPass && u.Role == "Admin").FirstOrDefaultAsync();
            if (user != null)
            {
                return new AdminInfo
                {
                    LoginResult = true,
                    Id = user.Id,
                    Username = user.AlbumNumber,
                    Password = user.Password,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    IsRegistered = user.IsRegistered,
                    IsBlocked = user.IsBlocked,
                    Initials = user.Initials,
                    ErrorUsername = null,
                    ErrorPassword = null
                };
            }
            else
                return new AdminInfo
                {
                    LoginResult = false,
                    ErrorPassword = "Nieprawidłowe hasło",
                    ErrorUsername = null
                };
            
        }

        public async Task<UserInfo> AuthenticateUser(UserInfo info)
        {
            UserInfo user = null;
            string cryptedPass = Cryptor.Encrypt(info.Password);
            UserInfo checkUser = null;
            if (info.AlbumNumber != null)
                checkUser = await context.Users.Where(u => u.AlbumNumber == info.AlbumNumber && u.Role == "Student").FirstOrDefaultAsync();
            else
                checkUser = await context.Users.Where(u => u.Login == info.Login && u.Role == "Student").FirstOrDefaultAsync();

            if (checkUser == null)
            {
                return new UserInfo
                {
                    LoginResult = false,
                    ErrorUsername = "Brak studenta o podanym loginie lub numerze albumu",
                    ErrorPassword = null,
                };
            }


            if (info.AlbumNumber != null)
                user = await context.Users.Where(u => u.AlbumNumber == info.AlbumNumber && u.Password == cryptedPass && u.Role == "Student").FirstOrDefaultAsync();
            else
                user = await context.Users.Where(u => u.Login == info.Login && u.Password == cryptedPass && u.Role == "Student").FirstOrDefaultAsync();


            if (user == null)
            {
                return new UserInfo
                {
                    LoginResult = false,
                    ErrorPassword = "Nieprawidłowe hasło",
                    ErrorUsername = null
                };
            }

            if (user.IsBlocked)
            {
                return new UserInfo
                {
                    LoginResult = false,
                    ErrorPassword = null,
                    ErrorUsername = "Konto użytkownika zostało zablokowane"
                };
            }
            else
            {
                user.LoginResult = true;
                user.ErrorUsername = null;
                user.ErrorPassword = null;
                return user;
            }
        }

        public async Task<string> GetUserPassword(string userId)
        {
            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user.Password;
        }

        public async Task<string> GetUserLogin(string userId)
        {
            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user.Login;
        }

        public async Task<string> GetUserGitHubLogin(string userId)
        {
            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user.GitHubLogin;
        }

        public async Task<OperationResult> ResetUserPassword(UserInfo info, string apiKey)
        {
            var checkUser = await context.Users.Where(u => u.AlbumNumber == info.AlbumNumber && u.Role == "Student").FirstOrDefaultAsync();
            if (checkUser == null)
            {
                return new OperationResult
                {
                    Result = false,
                    Error = "Brak studenta o podanym numerze albumu",
                    Content = null
                };
            }


            UserInfo user = await context.Users.Where(u => u.AlbumNumber == info.AlbumNumber && u.EmailAddress == info.EmailAddress && u.Role == "Student").FirstOrDefaultAsync();
            if (user == null)
            {
                return new OperationResult
                {
                    Result = false,
                    Error = "Adres email nie jest zgodny z numerem albumu studenta",
                    Content = null
                };
            }

            string newPassword = RandomPassword.GenerateTemporaryPassword();
            using (SendGridSender sender = new SendGridSender(apiKey))
            {
                var result = await sender.SendEmail_ResetUserPassword(user.EmailAddress, user.FirstName, user.LastName, newPassword);
                if (result.Result)
                {
                    await IncreaseSendGridAccount();
                    user.Password = Cryptor.Encrypt(newPassword);
                    await context.SaveChangesAsync();               
                }

                return result;
            }   
        }

        public async Task<int> GetSendGrid()
        {
            var sendGrids = await context.SendGrids.OrderByDescending(s => s.SendDate).ToListAsync();
            var lastDate = sendGrids.FirstOrDefault();
            if (lastDate == null)
                return 0;
            else
            {
                var today = DateTime.Now.ToString().Substring(0, 10);
                if (lastDate.DateComparer == today)
                    return lastDate.NumberOfSends;
                else
                    return 0;
            }
        }


        private async Task IncreaseSendGridAccount()
        {
            var sendGrids = context.SendGrids.OrderByDescending(s => s.SendDate).ToList();
            var lastDate = sendGrids.FirstOrDefault();
            if (lastDate == null)
            {
                await context.SendGrids.AddAsync(CreateNewSendGridInfo());
            }
            else
            {
                var today = DateTime.Now.ToString().Substring(0, 10);
                if (lastDate.DateComparer == today)
                {
                    lastDate.NumberOfSends++;
                }
                else
                {
                    await context.SendGrids.AddAsync(CreateNewSendGridInfo());
                }
            }

            await context.SaveChangesAsync();

        }

        private SendGridInfo CreateNewSendGridInfo()
        {
            SendGridInfo info = new SendGridInfo();
            info.Id = Guid.NewGuid().ToString();
            info.SendDate = DateTime.Now;
            info.NumberOfSends = 1;
            return info;
        }

       
    }
}
