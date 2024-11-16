using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class V3_AddActivitySelfRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "parent_activity_id",
                table: "activity",
                type: "bigint",
                nullable: true);

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
                name: "IX_activity_parent_activity_id",
                table: "activity",
                column: "parent_activity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_activity_activity_parent_activity_id",
                table: "activity",
                column: "parent_activity_id",
                principalTable: "activity",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_activity_activity_parent_activity_id",
                table: "activity");

            migrationBuilder.DropIndex(
                name: "IX_activity_parent_activity_id",
                table: "activity");

            migrationBuilder.DropColumn(
                name: "parent_activity_id",
                table: "activity");

            migrationBuilder.UpdateData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "password", "updated_date" },
                values: new object[] { new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(3932), "$2a$11$vX8uXeVsTwOLRKjs.GPiX.AVuMTD/Xy4IlfYtdjmfPmUxtfv8Z9Eq", new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(3965) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(4676), new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(4677) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "created_date", "updated_date" },
                values: new object[] { new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(4768), new DateTime(2024, 9, 30, 14, 59, 20, 561, DateTimeKind.Local).AddTicks(4769) });
        }
    }
}
