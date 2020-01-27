using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    NameIdentifier = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: false),
                    UserType = table.Column<int>(nullable: false),
                    UserStatus = table.Column<int>(nullable: false),
                    Department = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appUsers_UserName",
                table: "appUsers",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appUsers");
        }
    }
}
