using Microsoft.EntityFrameworkCore;
using StudentApi.Entities.Authorization;
using StudentApi.Entities.Data;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelStudentGroup>()
                .HasKey(c => new { c.GroupId, c.StudentId });
        }

        public DbSet<BasicInfoItem> InfoItems { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<GroupInfo> Groups { get; set; }
        public DbSet<RelStudentGroup> Relation_StudentGroup { get; set; }
        public DbSet<SendGridInfo> SendGrids { get; set; }
    }
}
