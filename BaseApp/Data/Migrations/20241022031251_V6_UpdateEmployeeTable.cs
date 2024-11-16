using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V6_UpdateEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_break_end",
                table: "company_info");

            migrationBuilder.DropColumn(
                name: "default_break_start",
                table: "company_info");

            migrationBuilder.AddColumn<string>(
                name: "github_name",
                table: "employee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "github_name", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7462), null, "$2a$11$YwDRYcVrPC/ZOSAOIWZOAueeaGpJXvqCU5LOCJuOhRJAPtkkFEb4u", new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7483) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7984), new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7985) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(8009), new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(8010) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "github_name",
                table: "employee");

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
    }
}
