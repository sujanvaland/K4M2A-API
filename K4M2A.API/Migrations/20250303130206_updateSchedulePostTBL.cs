using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace K4M2A.API.Migrations
{
    /// <inheritdoc />
    public partial class updateSchedulePostTBL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScheduled",
                schema: "dbo",
                table: "SchedulePost",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScheduled",
                schema: "dbo",
                table: "SchedulePost");
        }
    }
}
