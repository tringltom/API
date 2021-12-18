using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddedXpLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentLevel",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentXp",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "XpLevel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Xp = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XpLevel", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "XpLevel",
                columns: new[] { "Id", "Level", "Xp" },
                values: new object[,]
                {
                    { 1, 1, 0 },
                    { 18, 18, 153000 },
                    { 17, 17, 136000 },
                    { 16, 16, 120000 },
                    { 15, 15, 105000 },
                    { 14, 14, 91000 },
                    { 13, 13, 78000 },
                    { 12, 12, 66000 },
                    { 11, 11, 55000 },
                    { 10, 10, 45000 },
                    { 9, 9, 36000 },
                    { 8, 8, 28000 },
                    { 7, 7, 21000 },
                    { 6, 6, 15000 },
                    { 5, 5, 10000 },
                    { 4, 4, 6000 },
                    { 3, 3, 3000 },
                    { 2, 2, 1000 },
                    { 19, 19, 171000 },
                    { 20, 20, 190000 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XpLevel");

            migrationBuilder.DropColumn(
                name: "CurrentLevel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentXp",
                table: "AspNetUsers");
        }
    }
}
