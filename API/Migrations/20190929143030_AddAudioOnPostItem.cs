using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class AddAudioOnPostItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "PostItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audio",
                table: "PostItems");
        }
    }
}
