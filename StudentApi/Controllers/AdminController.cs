using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentApi.Entities.Authorization;
using StudentApi.Interfaces;

namespace StudentApi.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        #region Properties & Constructor

        private readonly IAdminService adminService;

        public AdminController(IAdminService admSrv)
        {
            adminService = admSrv;
        }
        #endregion


        #region Endpoints

        [Authorize]
        [HttpPost("/api/addstudent")]
        public async Task<IActionResult> AddStudentAsync()
        {
            UserInfo user;
            string body;
            IActionResult response = BadRequest(new { general = "Brak wymaganych danych" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                user = JsonConvert.DeserializeObject<UserInfo>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (user != null)
            {
                var result = await adminService.AddStudent(user.AlbumNumber, user.EmailAddress, user.FirstName, user.LastName);
                if (!result.Result)
                    response = BadRequest(new { general = result.Error });
                else
                    response = Ok(new { email =  user.EmailAddress, temporaryPassword  = result.Content });
            }

            return response;
        }

        [Authorize]
        [HttpGet("/api/students")]
        public async Task<string> GetAllStudents()
        {
            var data = await adminService.GetAllStudents();
            return JsonConvert.SerializeObject(data);
        }





        #endregion

    }
}