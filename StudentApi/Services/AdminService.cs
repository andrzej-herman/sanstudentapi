using Microsoft.EntityFrameworkCore;
using StudentApi.Entities.Authorization;
using StudentApi.Helpers;
using StudentApi.Interfaces;
using StudentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly SanStudentContext context;

        public AdminService(SanStudentContext ctx)
        {
            context = ctx;
        }

        public async Task<OperationResult> AddStudent(string albumNumber, string email, string firstName, string lastName)
        {
            OperationResult result = new OperationResult();
            string returnedPassword = RandomPassword.GenerateTemporaryPassword();

            // Check proper data
            if (!ValidateStudentData(albumNumber.Trim(), email.Trim(), firstName.Trim(), lastName.Trim(), out string error))
            {
                result.Result = false;
                result.Error = error;
                return result;
            }

            // Check existing data
            var checkAlbum = await context.Users.Where(u => u.AlbumNumber == albumNumber.Trim()).FirstOrDefaultAsync();
            if (checkAlbum != null)
            {
                result.Result = false;
                result.Error = "Podany numer albumu jest już zarejestrowany";
                return result;
            }

            var checkEmail = await context.Users.Where(u => u.EmailAddress == email.Trim()).FirstOrDefaultAsync();
            if (checkEmail != null)
            {
                result.Result = false;
                result.Error = "Podany adres email jest już zarejestrowany";
                return result;
            }

            UserInfo user = new UserInfo
            {
                Id = Guid.NewGuid().ToString(),
                AlbumNumber = albumNumber.Trim(),
                Password = Cryptor.Encrypt(returnedPassword),
                EmailAddress = email.Trim(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Role = "Student",
                IsRegistered = false,
                IsBlocked = false,
                DateCreated = DateTime.Now,
                DateBlocked = null,
                DateRegistered = null
            };

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                result.Result = true;
                result.Error = null;
                result.Content = returnedPassword;
                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Error = ex.ToString();
                return result;
            }
        }

        public async Task<List<Student>> GetAllStudents()
        {
            List<Student> res = new List<Student>();
            var users = await context.Users.Where(u => u.Role == "Student").ToListAsync();
            foreach (var user in users)
            {
                res.Add(
                   new Student
                   {
                        Id = user.Id,
                        AlbumNumber= user.AlbumNumber,
                        Password = user.IsRegistered ? "**********" : Cryptor.Decrypt(user.Password),
                        EmailAddress = user.EmailAddress,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsRegistered = user.IsRegistered,
                        IsBlocked = user.IsBlocked,
                        DateCreated = user.DateCreated.Value,
                        DateRegistered = user.DateRegistered,
                        DateBlocked = user.DateBlocked
                    });
            }

            return res.OrderBy(s => s.LastName).ToList();
        }

        private bool ValidateStudentData(string albumNumber, string email, string firstName, string lastName, out string error)
        {
            error = string.Empty;
            bool res = true;
            // AlbumNumber
            if (string.IsNullOrEmpty(albumNumber))
            {
                error = "Nieprawidłowy numer albumu";
                return false;
            }

            if (albumNumber.Length < 5 || albumNumber.Length > 6)
            {
                error = "Nieprawidłowy numer albumu";
                return false;
            }

            if (!IsNumeric(albumNumber))
            {
                error = "Nieprawidłowy numer albumu";
                return false;
            }

            // Email
            if (string.IsNullOrEmpty(email))
            {
                error = "Nieprawidłowy adres email";
                return false;
            }

            if (!IsValidEmail(email))
            {
                error = "Nieprawidłowy adres email";
                return false;
            }

            // FirstName
            if (string.IsNullOrEmpty(firstName))
            {
                error = "Nieprawidłowe imię";
                return false;
            }

            // LastName
            if (string.IsNullOrEmpty(lastName))
            {
                error = "Nieprawidłowe nazwisko";
                return false;
            }

            return res;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsNumeric(string value)
        {
            bool checkResult = true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value, i))
                {
                    checkResult = false;
                    break;
                }
            }
            return checkResult;
        }

        
    }
}
