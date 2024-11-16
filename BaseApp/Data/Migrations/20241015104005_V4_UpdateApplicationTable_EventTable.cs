using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V4_UpdateApplicationTable_EventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_employee_EmployeeId",
                table: "application");

            migrationBuilder.DropIndex(
                name: "IX_application_EmployeeId",
                table: "application");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "application");

            migrationBuilder.RenameColumn(
                name: "event_date",
                table: "event",
                newName: "start_date");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "event",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "application",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8146), "$2a$11$2b3QJPv.4OnP7D2m3bV18.3MmDMfwFyNWL64fz0wm/f4GrkWl1XBS", new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8175) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8593), new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8594) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8618), new DateTime(2024, 10, 15, 17, 40, 4, 719, DateTimeKind.Local).AddTicks(8619) });

            migrationBuilder.CreateIndex(
                name: "IX_application_emp_id",
                table: "application",
                column: "emp_id");

            migrationBuilder.AddForeignKey(
                name: "FK_application_employee_emp_id",
                table: "application",
                column: "emp_id",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_employee_emp_id",
                table: "application");

            migrationBuilder.DropIndex(
                name: "IX_application_emp_id",
                table: "application");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "event");

            migrationBuilder.DropColumn(
                name: "note",
                table: "application");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "event",
                newName: "event_date");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "application",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(1929), "$2a$11$xvaEz8GZweg5YqtlWaEreuI2EDTE1Lq1SoTQMtveS1RzIRL6u0OrO", new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(1978) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(2610), new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(2612) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(2655), new DateTime(2024, 10, 9, 8, 5, 42, 210, DateTimeKind.Local).AddTicks(2657) });

            migrationBuilder.CreateIndex(
                name: "IX_application_EmployeeId",
                table: "application",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_application_employee_EmployeeId",
                table: "application",
                column: "EmployeeId",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
