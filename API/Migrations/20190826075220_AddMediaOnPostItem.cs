using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class AddMediaOnPostItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Media",
                table: "PostItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Media",
                table: "PostItems");
        }
    }
}
