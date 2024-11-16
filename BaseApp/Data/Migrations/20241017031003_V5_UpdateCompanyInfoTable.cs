using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V5_UpdateCompanyInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "default_break_end",
                table: "company_info",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "default_break_start",
                table: "company_info",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(8886), "$2a$11$aGl5cwNcmZt00TuLypji5u9ZsQt6BohXOXuJCEYs9cqvEfRIFkbz.", new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(8924) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(9753), new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(9755) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(9785), new DateTime(2024, 10, 17, 10, 10, 1, 606, DateTimeKind.Local).AddTicks(9786) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_break_end",
                table: "company_info");

            migrationBuilder.DropColumn(
                name: "default_break_start",
                table: "company_info");

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
        }
    }
}
