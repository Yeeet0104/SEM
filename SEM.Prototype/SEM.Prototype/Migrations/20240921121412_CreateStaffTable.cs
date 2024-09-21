using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEM.Prototype.Migrations
{
    /// <inheritdoc />
    public partial class CreateStaffTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "AvailableEndTime",
                table: "StaffAvailability");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Staff");

            migrationBuilder.RenameTable(
                name: "StaffAvailability",
                newName: "StaffAvailabilities");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "Staffs");

            migrationBuilder.RenameColumn(
                name: "StaffName",
                table: "StaffAvailabilities",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "AvailableStartTime",
                table: "StaffAvailabilities",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Staffs",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffAvailabilities",
                table: "StaffAvailabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAvailabilities_StaffId",
                table: "StaffAvailabilities",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAvailabilities_Staffs_StaffId",
                table: "StaffAvailabilities",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffAvailabilities_Staffs_StaffId",
                table: "StaffAvailabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffAvailabilities",
                table: "StaffAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_StaffAvailabilities_StaffId",
                table: "StaffAvailabilities");

            migrationBuilder.RenameTable(
                name: "Staffs",
                newName: "Staff");

            migrationBuilder.RenameTable(
                name: "StaffAvailabilities",
                newName: "StaffAvailability");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Staff",
                newName: "StaffId");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "StaffAvailability",
                newName: "StaffName");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "StaffAvailability",
                newName: "AvailableStartTime");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Staff",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AvailableEndTime",
                table: "StaffAvailability",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "StaffId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffAvailability",
                table: "StaffAvailability",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppointmentTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StaffId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments",
                column: "StaffId");
        }
    }
}
