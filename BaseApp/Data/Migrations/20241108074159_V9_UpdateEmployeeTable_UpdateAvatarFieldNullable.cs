using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V9_UpdateEmployeeTable_UpdateAvatarFieldNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "avatar",
                table: "employee",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(8530), "$2a$11$q6y.q5yjjRDFmtr0hKKWN.p5bQxIiwAgC8ukDu.I8veyDbv2FfFM.", new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(8564) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(9449), new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(9450) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(9481), new DateTime(2024, 11, 8, 14, 41, 58, 696, DateTimeKind.Local).AddTicks(9482) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "avatar",
                table: "employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
