using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentApi.Migrations
{
    public partial class AddSendGridsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SendGrids",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SendDate = table.Column<DateTime>(nullable: false),
                    NumberOfSends = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendGrids", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendGrids");
        }
    }
}
