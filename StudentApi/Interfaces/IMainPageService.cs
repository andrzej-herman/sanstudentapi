using StudentApi.Entities.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Interfaces
{
    public interface IMainPageService
    {
        Task<List<BasicInfoItem>> GetMainPageInfo();
    }
}
