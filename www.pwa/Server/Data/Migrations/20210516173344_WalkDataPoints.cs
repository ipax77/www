using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Data.Migrations
{
    public partial class WalkDataPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "wwwWalks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextPointId",
                table: "wwwWalks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WwwWalkData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    WwwWalkID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WwwWalkData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WwwWalkData_wwwWalks_WwwWalkID",
                        column: x => x.WwwWalkID,
                        principalTable: "wwwWalks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wwwWalks_NextPointId",
                table: "wwwWalks",
                column: "NextPointId");

            migrationBuilder.CreateIndex(
                name: "IX_WwwWalkData_WwwWalkID",
                table: "WwwWalkData",
                column: "WwwWalkID");

            migrationBuilder.AddForeignKey(
                name: "FK_wwwWalks_WwwWalkData_NextPointId",
                table: "wwwWalks",
                column: "NextPointId",
                principalTable: "WwwWalkData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wwwWalks_WwwWalkData_NextPointId",
                table: "wwwWalks");

            migrationBuilder.DropTable(
                name: "WwwWalkData");

            migrationBuilder.DropIndex(
                name: "IX_wwwWalks_NextPointId",
                table: "wwwWalks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "wwwWalks");

            migrationBuilder.DropColumn(
                name: "NextPointId",
                table: "wwwWalks");
        }
    }
}
