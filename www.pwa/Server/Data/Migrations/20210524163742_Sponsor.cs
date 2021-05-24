using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Data.Migrations
{
    public partial class Sponsor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntitySponsor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityID = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CentPerKm = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySponsor", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntitySponsor_wwwEntities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "wwwEntities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalkSponsor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WalkID = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CentPerKm = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalkSponsor", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WalkSponsor_wwwWalks_WalkID",
                        column: x => x.WalkID,
                        principalTable: "wwwWalks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntitySponsor_EntityID",
                table: "EntitySponsor",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_WalkSponsor_WalkID",
                table: "WalkSponsor",
                column: "WalkID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntitySponsor");

            migrationBuilder.DropTable(
                name: "WalkSponsor");
        }
    }
}
