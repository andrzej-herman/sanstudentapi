using Microsoft.EntityFrameworkCore;
using StudentApi.Interfaces;
using StudentApi.Entities.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Services
{
    public class MainPageService : IMainPageService
    {
        private readonly SanStudentContext context;

        public MainPageService(SanStudentContext ctx)
        {
            context = ctx;
        }

        public async Task<List<BasicInfoItem>> GetMainPageInfo()
        {
            var data = await context.InfoItems.OrderBy(it => it.Order).ToListAsync();
            if (data.Count > 0)
                return data;
            else
                return null;            
        }
    }
}
