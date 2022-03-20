using Microsoft.EntityFrameworkCore.Migrations;

namespace panelApi.Migrations
{
    public partial class ContactUpgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstagramAccount",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInAccount",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterAccount",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstagramAccount",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LinkedInAccount",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "TwitterAccount",
                table: "Contacts");
        }
    }
}
