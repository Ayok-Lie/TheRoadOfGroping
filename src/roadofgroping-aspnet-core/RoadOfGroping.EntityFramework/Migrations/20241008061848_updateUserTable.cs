using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadOfGroping.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "RoadOfGropingUsers",
                newName: "Roles");

            migrationBuilder.UpdateData(
                table: "RoadOfGropingUsers",
                keyColumn: "Id",
                keyValue: new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"),
                column: "Roles",
                value: "[\"Admin\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "RoadOfGropingUsers",
                newName: "Role");

            migrationBuilder.UpdateData(
                table: "RoadOfGropingUsers",
                keyColumn: "Id",
                keyValue: new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"),
                column: "Role",
                value: "Admin");
        }
    }
}
