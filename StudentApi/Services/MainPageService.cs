using Microsoft.EntityFrameworkCore;
using StudentApi.Interfaces;
using StudentApi.Entities.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentApi.Helpers;

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
           return await context.InfoItems.OrderBy(it => it.Order).ToListAsync();          
        }

        public async Task<OperationResult> UpdateMainPageInfo(List<BasicInfoItem> updatedList)
        {
            OperationResult result = new OperationResult();
            try
            {
                foreach (var item in updatedList)
                {
                    var info = await context.InfoItems.FirstOrDefaultAsync(i => i.Id == item.Id);
                    info.Content = item.Content;
                    info.Order = item.Order;
                    info.Color = item.Color;
                }

                await context.SaveChangesAsync();
                result.Result = true;
                result.Error = null;
                result.Content = "Informacje zostały zaktualizowane";
                return result;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Error = ex.ToString();
                result.Content = null;
                return result;
            }
        }


       

    }
}
