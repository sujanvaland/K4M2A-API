using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class addbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                schema: "dbo",
                table: "Book");

            migrationBuilder.AddColumn<string>(
                name: "BookId",
                schema: "dbo",
                table: "Book",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookId",
                schema: "dbo",
                table: "Book");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                schema: "dbo",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
