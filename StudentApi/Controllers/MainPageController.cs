using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentApi.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Text;
using StudentApi.Entities.MainPage;

namespace StudentApi.Controllers
{
    [ApiController]
    public class MainPageController : ControllerBase
    {
        private readonly IMainPageService mainPageService;

        public MainPageController(IMainPageService mpsrv)
        {
            mainPageService = mpsrv;
        }

        [HttpGet("/api/mainpageinfo")]
        public async Task<string> GetMainPageInfo()
        {
            var data = await mainPageService.GetMainPageInfo();
            return JsonConvert.SerializeObject(data);
        }
        
        [Authorize]
        [HttpPost("/api/updatemainpageinfo")]
        public async Task<IActionResult> UpdateMainPageInfo()
        {
            IActionResult response = BadRequest(new { general = "Wystąpił błąd podczas aktualizacji danych" });
            List<BasicInfoItem> data;
            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            try
            {
                data = JsonConvert.DeserializeObject<List<BasicInfoItem>>(body);
                foreach (var item in data)
                {
                    if (string.IsNullOrEmpty(item.Content.Trim()))
                        item.Content = null;
                }
            }                                                                           
            catch (Exception)
            {
                return response;
            }

            if (data != null)
            {
                var result = await mainPageService.UpdateMainPageInfo(data);
                if (!result.Result)
                    response = BadRequest(new { general = result.Error });
                else
                    response = Ok(new { general = result.Content });
            }

            return response;         
        }

    }
}
