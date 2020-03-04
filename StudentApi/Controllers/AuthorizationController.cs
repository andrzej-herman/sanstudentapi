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
        private readonly IConfiguration configuration;
        private readonly IAuthService authorizationService;

        public AuthorizationController(IConfiguration config, IAuthService authSrv)
        {
            configuration = config;
            authorizationService = authSrv;
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> LoginAsync()
        {
            UserInfo model;
            UserInfo user;
            string body;
            IActionResult response = BadRequest(new { general = "Nieprawidłowe dane logowania" });
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
                user = await authorizationService.AuthenticateUser(model);
                if (user != null)
                {
                    if (user.IsBlocked)
                        response = BadRequest(new { general = "Konto użytkownika zostało zablokowane" });
                    else
                    {
                        var tokenStr = GenerateJsonWebToken(user);
                        response = Ok(new { token = tokenStr });
                    }
                    
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
            // Get other values from Db (groups, absence, marks etc ...)

            return JsonConvert.SerializeObject(user);
        }

////[Authorize]
//[HttpPost("/api/addstudent")]
//public async Task<IActionResult> AddStudentAsync()
//{
//    UserInfo model;
//    UserInfo user;
//    string body;
//    IActionResult response = BadRequest(new { general = "Brak wymaganych danyh" });
//    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
//    {
//        body = await reader.ReadToEndAsync();
//    }

//    try
//    {
//        model = JsonConvert.DeserializeObject<UserInfo>(body);
//    }
//    catch (Exception)
//    {
//        return response;
//    }


//    //{
//    //    "albumnumber": "74800",
//    // "emailaddress": "test@test.pl",
//    // "firstname": "Marcin",
//    // "lastname": "Klupa"
//    //}



//    if (model != null)
//    {
//        user = await authorizationService.AuthenticateUser(model);
//        if (user != null)
//        {
//            var tokenStr = GenerateJsonWebToken(user);
//            response = Ok(new { token = tokenStr });
//        }
//    }

//    return response;
//}



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


}
}
 