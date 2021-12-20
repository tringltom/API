using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddedXpLevelRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentXp",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XpLevelId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "XpLevels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Xp = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XpLevels", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "XpLevels",
                columns: new[] { "Id", "Xp" },
                values: new object[,]
                {
                    { 1, 0 },
                    { 18, 153000 },
                    { 17, 136000 },
                    { 16, 120000 },
                    { 15, 105000 },
                    { 14, 91000 },
                    { 13, 78000 },
                    { 12, 66000 },
                    { 11, 55000 },
                    { 10, 45000 },
                    { 9, 36000 },
                    { 8, 28000 },
                    { 7, 21000 },
                    { 6, 15000 },
                    { 5, 10000 },
                    { 4, 6000 },
                    { 3, 3000 },
                    { 2, 1000 },
                    { 19, 171000 },
                    { 20, 190000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_XpLevelId",
                table: "AspNetUsers",
                column: "XpLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_XpLevels_XpLevelId",
                table: "AspNetUsers",
                column: "XpLevelId",
                principalTable: "XpLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_XpLevels_XpLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "XpLevels");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_XpLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentXp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "XpLevelId",
                table: "AspNetUsers");
        }
    }
}
