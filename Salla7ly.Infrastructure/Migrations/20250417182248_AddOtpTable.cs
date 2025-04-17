using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Salla7ly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserOtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOtps", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 4, 17, 18, 22, 47, 962, DateTimeKind.Utc).AddTicks(4825), "AQAAAAIAAYagAAAAEGn423AqdORNVbH8fv/aySmSfoh4301y3Hg55HYskmPc0QA2Hlaa0PS9x9WiSLkSWg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOtps");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 29, 23, 9, 53, 296, DateTimeKind.Utc).AddTicks(2169), "AQAAAAIAAYagAAAAEIfVs69SjOIqatna0qJh3jL40NpgCRa5LDSxnUY5XIYzz5zi2MIGX5t5w9IwE5Y05A==" });
        }
    }
}
