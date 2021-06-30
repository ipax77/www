using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Migrations
{
    public partial class WalkData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WwwWalkData_wwwWalks_WwwWalkID",
                table: "WwwWalkData");

            migrationBuilder.DropForeignKey(
                name: "FK_wwwWalks_WwwWalkData_NextPointId",
                table: "wwwWalks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WwwWalkData",
                table: "WwwWalkData");

            migrationBuilder.RenameTable(
                name: "WwwWalkData",
                newName: "wwwWalkDatas");

            migrationBuilder.RenameIndex(
                name: "IX_WwwWalkData_WwwWalkID",
                table: "wwwWalkDatas",
                newName: "IX_wwwWalkDatas_WwwWalkID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_wwwWalkDatas",
                table: "wwwWalkDatas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_wwwWalkDatas_wwwWalks_WwwWalkID",
                table: "wwwWalkDatas",
                column: "WwwWalkID",
                principalTable: "wwwWalks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_wwwWalks_wwwWalkDatas_NextPointId",
                table: "wwwWalks",
                column: "NextPointId",
                principalTable: "wwwWalkDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wwwWalkDatas_wwwWalks_WwwWalkID",
                table: "wwwWalkDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_wwwWalks_wwwWalkDatas_NextPointId",
                table: "wwwWalks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_wwwWalkDatas",
                table: "wwwWalkDatas");

            migrationBuilder.RenameTable(
                name: "wwwWalkDatas",
                newName: "WwwWalkData");

            migrationBuilder.RenameIndex(
                name: "IX_wwwWalkDatas_WwwWalkID",
                table: "WwwWalkData",
                newName: "IX_WwwWalkData_WwwWalkID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WwwWalkData",
                table: "WwwWalkData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WwwWalkData_wwwWalks_WwwWalkID",
                table: "WwwWalkData",
                column: "WwwWalkID",
                principalTable: "wwwWalks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_wwwWalks_WwwWalkData_NextPointId",
                table: "wwwWalks",
                column: "NextPointId",
                principalTable: "WwwWalkData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
