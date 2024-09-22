using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEM.Prototype.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableDayToStaffAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableDay",
                table: "StaffAvailability",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableDay",
                table: "StaffAvailability");
        }
    }
}
