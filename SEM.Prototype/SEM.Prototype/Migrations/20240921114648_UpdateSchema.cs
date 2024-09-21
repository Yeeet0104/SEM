using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEM.Prototype.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "StaffAvailability",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StaffAvailability",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StaffAvailability");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "StaffAvailability",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability",
                column: "StaffId");
        }
    }
}
