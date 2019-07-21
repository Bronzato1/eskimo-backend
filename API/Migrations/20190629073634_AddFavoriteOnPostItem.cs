using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class AddFavoriteOnPostItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorite",
                table: "PostItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorite",
                table: "PostItems");
        }
    }
}
