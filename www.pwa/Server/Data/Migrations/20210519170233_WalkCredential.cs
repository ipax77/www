using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Data.Migrations
{
    public partial class WalkCredential : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Credential",
                table: "wwwWalks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credential",
                table: "wwwWalks");
        }
    }
}
