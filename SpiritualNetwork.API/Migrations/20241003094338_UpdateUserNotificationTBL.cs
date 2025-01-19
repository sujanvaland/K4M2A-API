using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserNotificationTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsEmail",
                schema: "dbo",
                table: "UserNotification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPush",
                schema: "dbo",
                table: "UserNotification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSMS",
                schema: "dbo",
                table: "UserNotification",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmail",
                schema: "dbo",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "IsPush",
                schema: "dbo",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "IsSMS",
                schema: "dbo",
                table: "UserNotification");

        
        }
    }
}
