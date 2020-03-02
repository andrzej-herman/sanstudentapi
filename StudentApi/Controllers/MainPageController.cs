using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentApi.Interfaces;
using Newtonsoft.Json;

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
    }
}
