using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class deleteuserpostcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikesCount",
                schema: "dbo",
                table: "UserPosts");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                schema: "dbo",
                table: "UserPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                schema: "dbo",
                table: "UserPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                schema: "dbo",
                table: "UserPosts",
                type: "int",
                nullable: true);
        }
    }
}
