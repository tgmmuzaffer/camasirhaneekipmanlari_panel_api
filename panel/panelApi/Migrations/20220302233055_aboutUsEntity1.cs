using Microsoft.EntityFrameworkCore.Migrations;

namespace panelApi.Migrations
{
    public partial class aboutUsEntity1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUs");
        }
    }
}
