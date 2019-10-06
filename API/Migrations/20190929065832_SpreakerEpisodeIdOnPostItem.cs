using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class SpreakerEpisodeIdOnPostItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpreakerEpisodeId",
                table: "PostItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpreakerEpisodeId",
                table: "PostItems");
        }
    }
}
