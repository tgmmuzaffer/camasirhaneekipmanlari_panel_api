using Microsoft.EntityFrameworkCore.Migrations;

namespace panelApi.Migrations
{
    public partial class upgradeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPassword",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPassword",
                table: "Users");
        }
    }
}
