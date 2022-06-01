using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class ChallengeAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserChallengeAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChallengeAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChallengeAnswers_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChallengeAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeMedias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserChallengeAnswerId = table.Column<int>(nullable: true),
                    PublicId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallengeMedias_UserChallengeAnswers_UserChallengeAnswerId",
                        column: x => x.UserChallengeAnswerId,
                        principalTable: "UserChallengeAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeMedias_UserChallengeAnswerId",
                table: "ChallengeMedias",
                column: "UserChallengeAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengeAnswers_ActivityId",
                table: "UserChallengeAnswers",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengeAnswers_UserId",
                table: "UserChallengeAnswers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeMedias");

            migrationBuilder.DropTable(
                name: "UserChallengeAnswers");
        }
    }
}
