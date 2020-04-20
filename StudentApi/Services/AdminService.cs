using Microsoft.EntityFrameworkCore;
using StudentApi.Entities.Authorization;
using StudentApi.Entities.Data;
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

        public async Task<OperationResult> AddStudent(string albumNumber, string email, string firstName, string lastName, List<string> groups)
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
                foreach (var item in groups)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        await context.Relation_StudentGroup.AddAsync(new RelStudentGroup() { GroupId = item, StudentId = user.Id });
                    }
                }

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

        public async Task<OperationResult> AddGroup(GroupInfo group)
        {
            OperationResult result = new OperationResult();

            // Check proper data
            if (!ValidateGroupData(group, out string error))
            {
                result.Result = false;
                result.Error = error;
                return result;
            }

            // Check existing data
            var checkName = await context.Groups.Where(u => u.Name.ToLower() == group.Name.Trim().ToLower() && u.Subject.ToLower() == group.Subject.ToLower()).FirstOrDefaultAsync();
            if (checkName != null)
            {
                result.Result = false;
                result.Error = "Podana nazwa grupy już istnieje";
                return result;
            }

            // Check Time
            var checkTime = await context.Groups.Where(u => u.Year == group.Year.Trim() && u.Semester == group.Semester && u.Day == group.Day && u.Time == group.Time).FirstOrDefaultAsync();
            if (checkTime != null)
            {
                result.Result = false;
                result.Error = "Dzień i godzina są już zajęte dla innej grupy";
                return result;
            }


            group.Id = Guid.NewGuid().ToString();
            group.DateCreated = DateTime.Now;
            group.IsActive = true;

            try
            {
                await context.Groups.AddAsync(group);
                await context.SaveChangesAsync();
                result.Result = true;
                result.Error = null;
                result.Content = "Grupa studencka została pomyślnie utworzona";
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
                        DateBlocked = user.DateBlocked,
                        Login = user.Login,
                        GitHubLogin = user.GitHubLogin
                    });
            }

            return res.OrderBy(s => s.LastName).ToList();
        }

        public async Task<List<StudentGroup>> GetAllGroups()
        {
            List<StudentGroup> groups = new List<StudentGroup>();
            List<Student> students = await GetAllStudents();
            var groupsInfos = await context.Groups.ToListAsync();
            var ordered = groupsInfos.OrderBy(gi => gi.Day).ThenBy(gi => gi.Time).ToList();
            foreach (var g in ordered)
            {
                var ids = await context.Relation_StudentGroup.Where(r => r.GroupId == g.Id).Select(r => r.StudentId).ToListAsync();
                var ownedStudents = students.Where(s => ids.Contains(s.Id)).ToList();
                string season = g.Semester == Semester.SUMMER ? "letni" : "zimowy";
                string day = g.Day == Day.SATURDAY ? "sobota" : "niedziela";
                string lectureDay = g.LectureDay == Day.SATURDAY ? "sobota" : "niedziela";
                groups.Add(
                   new StudentGroup
                   {
                       Id = g.Id,
                       Season = $"{g.Year} {season}",
                       Name = $"{g.Subject} ({g.Name})",
                       Time = $"{day} ({g.Time})",
                       Lecture = $"{g.LecturerName} ({lectureDay}, {g.LectureTime})",
                       Students = ownedStudents,
                       DateCreated = g.DateCreated,
                       IsActive = g.IsActive,
                   });
            }

            return groups;
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

            if (!RandomPassword.IsNumeric(albumNumber))
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

       

        private bool ValidateGroupData(GroupInfo group, out string error)
        {
            error = "Nieprawidłowy rok akademicki";
            bool res = false;

            if (group.Year.Length == 9)
            {
               if (group.Year.Substring(4, 1) == "/")
                {
                    string[] data = group.Year.Split("/");
                    if (RandomPassword.IsNumeric(data[0]) && RandomPassword.IsNumeric(data[1]))
                    {
                        res = true;
                        error = string.Empty;
                    }
                }
            }

            return res;
        }

        
    }
}
