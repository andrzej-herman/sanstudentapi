using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StudentApi.Helpers;
using StudentApi.Interfaces;
using StudentApi.Entities.Authorization;
using StudentApi.Models;

namespace StudentApi.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        #region Properties & Constructor
        private readonly IConfiguration configuration;
        private readonly IAuthService authorizationService;

        public AuthorizationController(IConfiguration config, IAuthService authSrv)
        {
            configuration = config;
            authorizationService = authSrv;
        }
        #endregion

        #region Admin

        [HttpPost("/api/adminlogin")]
        public async Task<IActionResult> AdminLoginAsync()
        {
            AdminInfo model;
            AdminInfo admin;
            string body;
            IActionResult response = BadRequest(new { username = "Błąd połączenia z API", password = "Błąd połączenia z API" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                model = JsonConvert.DeserializeObject<AdminInfo>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (model != null)
            {
                admin = await authorizationService.AuthenticateAdmin(model);
                if (admin.LoginResult)
                {
                    var tokenStr = GenerateAdminJsonWebToken(admin);
                    response = Ok(new { token = tokenStr });
                }
                else
                {
                    response = BadRequest(new { username = admin.ErrorUsername, password = admin.ErrorPassword });
                }
                
            }

            return response;
        }

        [Authorize]
        [HttpGet("/api/getadmin")]
        public async Task<string> GetAdmin()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            string id = claim[0].Value;
            AdminModel admin = new AdminModel
            {
                DisplayName = "Andrzej Herman",
                Initials = "AH"
            };

            admin.Password = await authorizationService.GetUserPassword(id);
            return JsonConvert.SerializeObject(admin);
        }

        #endregion

        #region Student

        [HttpPost("/api/login")]
        public async Task<IActionResult> LoginAsync()
        {
            UserInfo model;
            UserInfo user;
            string body;
            IActionResult response = BadRequest(new { username = "Błąd połączenia z API", password = "Błąd połączenia z API" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                model = JsonConvert.DeserializeObject<UserInfo>(body);
                string data = model.AlbumNumber;
                if (!RandomPassword.IsNumeric(data))
                {
                    model.AlbumNumber = null;
                    model.Login = data;
                }
            }
            catch (Exception)
            {
                return response;
            }

            if (model != null)
            {
                user = await authorizationService.AuthenticateUser(model);
                if (user.LoginResult)
                {
                    var tokenStr = GenerateJsonWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                else
                {
                    response = BadRequest(new { username = user.ErrorUsername, password = user.ErrorPassword });
                }
            }

            return response;
        }

        [Authorize]
        [HttpGet("/api/getuser")]
        public async Task<string> GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            UserModel user = new UserModel
            {
                Id = claim[0].Value,
                AlbumNumber = claim[1].Value,
                FirstName = claim[2].Value,
                LastName = claim[3].Value,
                Initials = claim[4].Value,
                Email = claim[5].Value,
                Role = claim[6].Value,
                IsRegistered = Convert.ToBoolean(claim[7].Value),
                IsBlocked = Convert.ToBoolean(claim[8].Value),
            };

            user.Password = await authorizationService.GetUserPassword(user.Id);
            user.Login = await authorizationService.GetUserLogin(user.Id);
            user.GitHubLogin = await authorizationService.GetUserGitHubLogin(user.Id);
            // Get other values from Db (groups, absence, marks etc ...)

            return JsonConvert.SerializeObject(user);
        }


        [HttpPost("/api/resetpassword")]
        public async Task<IActionResult> ResetUserPassword()
        {
            UserInfo model;
            string body;
            IActionResult response = BadRequest(new { error = "Błąd połączenia z API" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                model = JsonConvert.DeserializeObject<UserInfo>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (model != null)
            {
                string apiKey = configuration["SendGrid:Key"];
                var result = await authorizationService.ResetUserPassword(model, apiKey);
                if (result.Result)
                {
                    response = Ok(new { content = result.Content });
                }
                else
                {
                    response = BadRequest(new {error = result.Error });
                }
            }

            return response;
        }


        #endregion

        #region Private
        private string GenerateJsonWebToken(UserInfo userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, userinfo.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, userinfo.AlbumNumber),
                new Claim(JwtRegisteredClaimNames.GivenName, userinfo.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, userinfo.LastName),
                new Claim(JwtRegisteredClaimNames.Acr, userinfo.Initials),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Typ, userinfo.Role),
                new Claim(JwtRegisteredClaimNames.Iat, userinfo.IsRegistered.ToString()),
                new Claim(JwtRegisteredClaimNames.Prn, userinfo.IsBlocked.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(

                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        private string GenerateAdminJsonWebToken(AdminInfo admininfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, admininfo.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, admininfo.Username),
                new Claim(JwtRegisteredClaimNames.GivenName, admininfo.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, admininfo.LastName),
                new Claim(JwtRegisteredClaimNames.Acr, admininfo.Initials),
                new Claim(JwtRegisteredClaimNames.Email, admininfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Typ, admininfo.Role),
                new Claim(JwtRegisteredClaimNames.Iat, admininfo.IsRegistered.ToString()),
                new Claim(JwtRegisteredClaimNames.Prn, admininfo.IsBlocked.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(

                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        } 
        #endregion
    }
}
 
 