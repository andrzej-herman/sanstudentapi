using StudentApi.Entities.MainPage;
using StudentApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Interfaces
{
    public interface IMainPageService
    {
        Task<List<BasicInfoItem>> GetMainPageInfo();

        Task<OperationResult> UpdateMainPageInfo(List<BasicInfoItem> updatedList);

    }
}
