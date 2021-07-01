using Microsoft.EntityFrameworkCore.Migrations;

namespace www.pwa.Server.Migrations
{
    public partial class ImageCopyRight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageCopyRightLink",
                table: "wwwWalkDatas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCopyRightLink",
                table: "wwwWalkDatas");
        }
    }
}
