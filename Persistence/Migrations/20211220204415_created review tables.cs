using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class createdreviewtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteActivities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityReviewXp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    ReviewTypeId = table.Column<int>(nullable: false),
                    Xp = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityReviewXp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityReviewXp_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityReviewXp_ReviewTypes_ReviewTypeId",
                        column: x => x.ReviewTypeId,
                        principalTable: "ReviewTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: false),
                    ReviewTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReviews_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviews_ReviewTypes_ReviewTypeId",
                        column: x => x.ReviewTypeId,
                        principalTable: "ReviewTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ReviewTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "None" },
                    { 2, "Poor" },
                    { 3, "Good" },
                    { 4, "Awesome" }
                });

            migrationBuilder.InsertData(
                table: "ActivityReviewXp",
                columns: new[] { "Id", "ActivityTypeId", "ReviewTypeId", "Xp" },
                values: new object[,]
                {
                    { 1, 1, 1, 0 },
                    { 16, 4, 4, 40 },
                    { 12, 3, 4, 50 },
                    { 8, 2, 4, 40 },
                    { 4, 1, 4, 200 },
                    { 23, 6, 3, 100 },
                    { 19, 5, 3, 100 },
                    { 15, 4, 3, 20 },
                    { 11, 3, 3, 20 },
                    { 7, 2, 3, 10 },
                    { 3, 1, 3, 50 },
                    { 22, 6, 2, 10 },
                    { 18, 5, 2, 10 },
                    { 14, 4, 2, 0 },
                    { 10, 3, 2, 0 },
                    { 6, 2, 2, 0 },
                    { 2, 1, 2, 20 },
                    { 21, 6, 1, 0 },
                    { 17, 5, 1, 0 },
                    { 13, 4, 1, 0 },
                    { 9, 3, 1, 0 },
                    { 5, 2, 1, -10 },
                    { 20, 5, 4, 200 },
                    { 24, 6, 4, 250 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityReviewXp_ActivityTypeId",
                table: "ActivityReviewXp",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityReviewXp_ReviewTypeId_ActivityTypeId",
                table: "ActivityReviewXp",
                columns: new[] { "ReviewTypeId", "ActivityTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteActivities_ActivityId",
                table: "UserFavoriteActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteActivities_UserId_ActivityId",
                table: "UserFavoriteActivities",
                columns: new[] { "UserId", "ActivityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ActivityId",
                table: "UserReviews",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ReviewTypeId",
                table: "UserReviews",
                column: "ReviewTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId_ActivityId",
                table: "UserReviews",
                columns: new[] { "UserId", "ActivityId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityReviewXp");

            migrationBuilder.DropTable(
                name: "UserFavoriteActivities");

            migrationBuilder.DropTable(
                name: "UserReviews");

            migrationBuilder.DropTable(
                name: "ReviewTypes");
        }
    }
}
