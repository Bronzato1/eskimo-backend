using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class ImagesForCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Categories",
                newName: "ImageText");

            migrationBuilder.AddColumn<string>(
                name: "ImageAudio",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageVideo",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAudio",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageVideo",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ImageText",
                table: "Categories",
                newName: "Image");
        }
    }
}
