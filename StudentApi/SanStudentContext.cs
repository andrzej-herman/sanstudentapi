using Microsoft.EntityFrameworkCore;
using StudentApi.Entities.Authorization;
using StudentApi.Entities.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi
{
    public class SanStudentContext : DbContext
    {
        public SanStudentContext(DbContextOptions options) : base(options) { }
        protected SanStudentContext() { }
        public DbSet<BasicInfoItem> InfoItems { get; set; }
        public DbSet<UserInfo> Users { get; set; }
    }
}
