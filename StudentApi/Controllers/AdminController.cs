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
using StudentApi.Entities.Data;
using StudentApi.Interfaces;
using StudentApi.Models;

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


        #region Students

        [Authorize]
        [HttpPost("/api/addstudent")]
        public async Task<IActionResult> AddStudentAsync()
        {
            UserInfo user;
            Obj_Groups objgroups;
            string body;
            IActionResult response = BadRequest(new { general = "Brak wymaganych danych" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                user = JsonConvert.DeserializeObject<UserInfo>(body);
                objgroups = JsonConvert.DeserializeObject<Obj_Groups>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (user != null && objgroups != null)
            {
                var result = await adminService.AddStudent(user.AlbumNumber, user.EmailAddress, user.FirstName, user.LastName, objgroups.groups);
                if (!result.Result)
                    response = BadRequest(new { general = result.Error });
                else
                    response = Ok(new { email = user.EmailAddress, temporaryPassword = result.Content });
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


        #region Groups
        [Authorize]
        [HttpPost("/api/addgroup")]
        public async Task<IActionResult> AddGroupAsync()
        {
            GroupInfo info;
            string body;
            IActionResult response = BadRequest(new { error = "Wystąpił błąd podczas aktualizacji danych grup" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                info = JsonConvert.DeserializeObject<GroupInfo>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (info != null)
            {
                var result = await adminService.AddGroup(info);
                if (!result.Result)
                    response = BadRequest(new { error = result.Error });
                else
                    response = Ok(new { text = result.Content });
            }

            return response;
        }


        [Authorize]
        [HttpGet("/api/groups")]
        public async Task<string> GetAllGroups()
        {
            var data = await adminService.GetAllGroups();
            return JsonConvert.SerializeObject(data);
        }

        [Authorize]
        [HttpPost("/api/changegroupstatus")]
        public async Task<IActionResult> ChangeGroupStatus()
        {
            string body;
            IActionResult response = BadRequest(new { error = "Wystąpił błąd podczas aktualizacji danych grupy" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            if (body == null)
                return response;

            var result = await adminService.ChangeGroupStatus(body);
            if (!result.Result)
                response = BadRequest(new { error = result.Error });
            else
                response = Ok(new { text = result.Content });


            return response;
        }


        [Authorize]
        [HttpPost("/api/addstudenttogroup")]
        public async Task<IActionResult> AddStudentToGroup()
        {
            string body;
            AddRemoveStudentToGroupModel model;
            IActionResult response = BadRequest(new { error = "Wystąpił błąd podczas aktualizacji danych grupy" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                model = JsonConvert.DeserializeObject<AddRemoveStudentToGroupModel>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (model != null)
            {
                var result = await adminService.AddStudentToGroup(model);
                if (!result.Result)
                    response = BadRequest(new { error = result.Error });
                else
                    response = Ok(new { text = result.Content });
            }

            return response;
        }

        [Authorize]
        [HttpPost("/api/removestudentfromgroup")]
        public async Task<IActionResult> RemoveStudentToGroup()
        {
            string body;
            AddRemoveStudentToGroupModel model;
            IActionResult response = BadRequest(new { error = "Wystąpił błąd podczas aktualizacji danych grupy" });
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                model = JsonConvert.DeserializeObject<AddRemoveStudentToGroupModel>(body);
            }
            catch (Exception)
            {
                return response;
            }

            if (model != null)
            {
                var result = await adminService.RemoveStudentFromGroup(model);
                if (!result.Result)
                    response = BadRequest(new { error = result.Error });
                else
                    response = Ok(new { text = result.Content });
            }

            return response;
        }

        #endregion




    }
}