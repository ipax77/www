using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Migrations
{
    public partial class LongDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageCopyRight",
                table: "WwwWalkData",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "WwwWalkData",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCopyRight",
                table: "WwwWalkData");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "WwwWalkData");
        }
    }
}
