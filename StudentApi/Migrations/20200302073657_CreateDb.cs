using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentApi.Migrations
{
    public partial class CreateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoItems");
        }
    }
}
