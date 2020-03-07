using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentApi.Migrations
{
    public partial class AddStudentGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Year = table.Column<string>(nullable: true),
                    Semester = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    Day = table.Column<int>(nullable: false),
                    Time = table.Column<string>(nullable: true),
                    LectureDay = table.Column<int>(nullable: false),
                    LectureTime = table.Column<string>(nullable: true),
                    LecturerName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relation_StudentGroup",
                columns: table => new
                {
                    GroupId = table.Column<string>(nullable: false),
                    StudentId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relation_StudentGroup", x => new { x.GroupId, x.StudentId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Relation_StudentGroup");
        }
    }
}
