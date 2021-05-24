using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Data.Migrations
{
    public partial class Sponsor3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntitySponsor_wwwEntities_EntityID",
                table: "EntitySponsor");

            migrationBuilder.DropForeignKey(
                name: "FK_WalkSponsor_wwwWalks_WalkID",
                table: "WalkSponsor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalkSponsor",
                table: "WalkSponsor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntitySponsor",
                table: "EntitySponsor");

            migrationBuilder.RenameTable(
                name: "WalkSponsor",
                newName: "walkSponsors");

            migrationBuilder.RenameTable(
                name: "EntitySponsor",
                newName: "entitySponsors");

            migrationBuilder.RenameIndex(
                name: "IX_WalkSponsor_WalkID",
                table: "walkSponsors",
                newName: "IX_walkSponsors_WalkID");

            migrationBuilder.RenameIndex(
                name: "IX_EntitySponsor_EntityID",
                table: "entitySponsors",
                newName: "IX_entitySponsors_EntityID");

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "walkSponsors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "entitySponsors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_walkSponsors",
                table: "walkSponsors",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_entitySponsors",
                table: "entitySponsors",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_entitySponsors_wwwEntities_EntityID",
                table: "entitySponsors",
                column: "EntityID",
                principalTable: "wwwEntities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_walkSponsors_wwwWalks_WalkID",
                table: "walkSponsors",
                column: "WalkID",
                principalTable: "wwwWalks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_entitySponsors_wwwEntities_EntityID",
                table: "entitySponsors");

            migrationBuilder.DropForeignKey(
                name: "FK_walkSponsors_wwwWalks_WalkID",
                table: "walkSponsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_walkSponsors",
                table: "walkSponsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_entitySponsors",
                table: "entitySponsors");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "walkSponsors");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "entitySponsors");

            migrationBuilder.RenameTable(
                name: "walkSponsors",
                newName: "WalkSponsor");

            migrationBuilder.RenameTable(
                name: "entitySponsors",
                newName: "EntitySponsor");

            migrationBuilder.RenameIndex(
                name: "IX_walkSponsors_WalkID",
                table: "WalkSponsor",
                newName: "IX_WalkSponsor_WalkID");

            migrationBuilder.RenameIndex(
                name: "IX_entitySponsors_EntityID",
                table: "EntitySponsor",
                newName: "IX_EntitySponsor_EntityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalkSponsor",
                table: "WalkSponsor",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntitySponsor",
                table: "EntitySponsor",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EntitySponsor_wwwEntities_EntityID",
                table: "EntitySponsor",
                column: "EntityID",
                principalTable: "wwwEntities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalkSponsor_wwwWalks_WalkID",
                table: "WalkSponsor",
                column: "WalkID",
                principalTable: "wwwWalks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
