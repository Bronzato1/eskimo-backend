using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class YoutubeVideoIdOnPostItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoId",
                table: "PostItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeVideoId",
                table: "PostItems");
        }
    }
}
