using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "counterQueues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CounterName = table.Column<string>(type: "TEXT", nullable: true),
                    ValueChange = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counterQueues", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "wwwCounters",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwCounters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "wwwWalks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TotalDistance = table.Column<float>(type: "REAL", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalRuns = table.Column<float>(type: "REAL", nullable: false),
                    TotalEntities = table.Column<int>(type: "INTEGER", nullable: false),
                    isActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwWalks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "wwwSchools",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TotalRuns = table.Column<float>(type: "REAL", nullable: false),
                    TotalEntities = table.Column<int>(type: "INTEGER", nullable: false),
                    WwwWalkID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwSchools", x => x.ID);
                    table.ForeignKey(
                        name: "FK_wwwSchools_wwwWalks_WwwWalkID",
                        column: x => x.WwwWalkID,
                        principalTable: "wwwWalks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wwwClasses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalRuns = table.Column<float>(type: "REAL", nullable: false),
                    TotalEntities = table.Column<int>(type: "INTEGER", nullable: false),
                    WwwSchoolID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwClasses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_wwwClasses_wwwSchools_WwwSchoolID",
                        column: x => x.WwwSchoolID,
                        principalTable: "wwwSchools",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wwwEntities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pseudonym = table.Column<string>(type: "TEXT", nullable: true),
                    TotalRuns = table.Column<float>(type: "REAL", nullable: false),
                    Runs = table.Column<int>(type: "INTEGER", nullable: false),
                    WwwClassID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwEntities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_wwwEntities_wwwClasses_WwwClassID",
                        column: x => x.WwwClassID,
                        principalTable: "wwwClasses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wwwRuns",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Distance = table.Column<float>(type: "REAL", nullable: false),
                    WwwEntityID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wwwRuns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_wwwRuns_wwwEntities_WwwEntityID",
                        column: x => x.WwwEntityID,
                        principalTable: "wwwEntities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wwwClasses_WwwSchoolID",
                table: "wwwClasses",
                column: "WwwSchoolID");

            migrationBuilder.CreateIndex(
                name: "IX_wwwEntities_WwwClassID",
                table: "wwwEntities",
                column: "WwwClassID");

            migrationBuilder.CreateIndex(
                name: "IX_wwwRuns_WwwEntityID",
                table: "wwwRuns",
                column: "WwwEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_wwwSchools_WwwWalkID",
                table: "wwwSchools",
                column: "WwwWalkID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counterQueues");

            migrationBuilder.DropTable(
                name: "wwwCounters");

            migrationBuilder.DropTable(
                name: "wwwRuns");

            migrationBuilder.DropTable(
                name: "wwwEntities");

            migrationBuilder.DropTable(
                name: "wwwClasses");

            migrationBuilder.DropTable(
                name: "wwwSchools");

            migrationBuilder.DropTable(
                name: "wwwWalks");
        }
    }
}
