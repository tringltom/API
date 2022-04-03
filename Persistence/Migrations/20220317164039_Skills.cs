using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class Skills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SkillSpecialId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SkillActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(nullable: false),
                    Counter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Skills_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityTypeOneId = table.Column<int>(nullable: false),
                    ActivityTypeTwoId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillSpecials_ActivityTypes_ActivityTypeOneId",
                        column: x => x.ActivityTypeOneId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillSpecials_ActivityTypes_ActivityTypeTwoId",
                        column: x => x.ActivityTypeTwoId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillXpBonuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(nullable: false),
                    Multiplier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillXpBonuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SkillActivities",
                columns: new[] { "Id", "Counter", "Level" },
                values: new object[,]
                {
                    { 1, 2, 0 },
                    { 2, 3, 1 },
                    { 3, 4, 2 },
                    { 4, 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "SkillSpecials",
                columns: new[] { "Id", "ActivityTypeOneId", "ActivityTypeTwoId", "Title" },
                values: new object[,]
                {
                    { 21, 5, 6, "Aktivist" },
                    { 20, 4, 6, "Šibicar" },
                    { 19, 4, 5, "Mađioničar" },
                    { 18, 3, 6, "Životni trener" },
                    { 17, 3, 5, "Glasnik" },
                    { 16, 3, 4, "Smarač" },
                    { 15, 2, 6, "Vragolan" },
                    { 14, 2, 5, "Komičar" },
                    { 13, 2, 4, "Enigmatičar" },
                    { 12, 2, 3, "Aforizmičar" },
                    { 11, 1, 6, "Pravednik" },
                    { 10, 1, 5, "Redar" },
                    { 9, 1, 4, "Profesor" },
                    { 8, 1, 3, "Savetnik" },
                    { 7, 1, 2, "Veseljak" },
                    { 6, 6, null, "Izazivač" },
                    { 5, 5, null, "Inicijator" },
                    { 4, 4, null, "Mozgalo" },
                    { 3, 3, null, "Filozof" },
                    { 2, 2, null, "Šaljivdžija" },
                    { 1, 1, null, "Dobrica" }
                });

            migrationBuilder.InsertData(
                table: "SkillXpBonuses",
                columns: new[] { "Id", "Level", "Multiplier" },
                values: new object[,]
                {
                    { 1, 0, 1 },
                    { 2, 1, 2 },
                    { 3, 2, 3 },
                    { 4, 3, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SkillSpecialId",
                table: "AspNetUsers",
                column: "SkillSpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_ActivityTypeId",
                table: "Skills",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_UserId",
                table: "Skills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillSpecials_ActivityTypeOneId",
                table: "SkillSpecials",
                column: "ActivityTypeOneId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillSpecials_ActivityTypeTwoId",
                table: "SkillSpecials",
                column: "ActivityTypeTwoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SkillSpecials_SkillSpecialId",
                table: "AspNetUsers",
                column: "SkillSpecialId",
                principalTable: "SkillSpecials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SkillSpecials_SkillSpecialId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SkillActivities");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "SkillSpecials");

            migrationBuilder.DropTable(
                name: "SkillXpBonuses");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SkillSpecialId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SkillSpecialId",
                table: "AspNetUsers");
        }
    }
}
