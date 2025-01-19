using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class inviterequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "dbo",
                table: "InviteRequest",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Journey",
                schema: "dbo",
                table: "InviteRequest",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "InviteRequest",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "dbo",
                table: "InviteRequest",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                schema: "dbo",
                table: "InviteRequest");

            migrationBuilder.DropColumn(
                name: "Journey",
                schema: "dbo",
                table: "InviteRequest");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "dbo",
                table: "InviteRequest");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "dbo",
                table: "InviteRequest");
        }
    }
}
