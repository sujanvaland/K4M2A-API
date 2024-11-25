using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class updateBookSTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                schema: "dbo",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "dbo",
                table: "Book",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                schema: "dbo",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Book");
        }
    }
}
