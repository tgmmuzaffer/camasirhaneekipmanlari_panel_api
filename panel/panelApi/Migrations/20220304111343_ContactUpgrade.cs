using Microsoft.EntityFrameworkCore.Migrations;

namespace panelApi.Migrations
{
    public partial class ContactUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactContent",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTitle",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDay1",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDay2",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDay3",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkHour3",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactContent",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactTitle",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "WorkDay1",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "WorkDay2",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "WorkDay3",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "WorkHour3",
                table: "Contacts");
        }
    }
}
