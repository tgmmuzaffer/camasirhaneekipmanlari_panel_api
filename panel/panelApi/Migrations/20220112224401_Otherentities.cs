using Microsoft.EntityFrameworkCore.Migrations;

namespace panelApi.Migrations
{
    public partial class Otherentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PropertyTabs");

            migrationBuilder.AddColumn<int>(
                name: "ProductPropertyId",
                table: "PropertyTabs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPropertyId",
                table: "PropertyTabs");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PropertyTabs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
