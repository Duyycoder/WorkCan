using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V8_UpdateCompanyInfoTable_AddBreakStartAndBreakend : Migration
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
                values: new object[] { new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(2581), "$2a$11$NnEVtomb4gLMeQKp5QcRLuIDZpaaUb4Z5wJbC7bpTPnvZ2iOYOJme", new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(2652) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(3483), new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(3484) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(3514), new DateTime(2024, 11, 8, 11, 10, 18, 352, DateTimeKind.Local).AddTicks(3514) });
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
                values: new object[] { new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(5992), "$2a$11$YHzIHhUUoKu12q6Gk93HIOM0DW8CoN6a5HeeFC/HqdcwPM0/ctLky", new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(6036) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(6727), new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(6728) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(6760), new DateTime(2024, 11, 1, 14, 15, 9, 254, DateTimeKind.Local).AddTicks(6761) });
        }
    }
}
