using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V7_UpdateCodeFieldInEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employee_code",
                table: "employee");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "employee",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.CreateIndex(
                name: "IX_employee_code",
                table: "employee",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employee_code",
                table: "employee");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "employee",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7462), "$2a$11$YwDRYcVrPC/ZOSAOIWZOAueeaGpJXvqCU5LOCJuOhRJAPtkkFEb4u", new DateTime(2024, 10, 22, 10, 12, 49, 879, DateTimeKind.Local).AddTicks(7483) });

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

            migrationBuilder.CreateIndex(
                name: "IX_employee_code",
                table: "employee",
                column: "code",
                unique: true);
        }
    }
}
