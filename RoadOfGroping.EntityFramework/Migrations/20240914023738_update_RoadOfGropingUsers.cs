using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadOfGroping.EntityFramework.Migrations
{
    public partial class update_RoadOfGropingUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "RoadOfGropingUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "RoadOfGropingUsers",
                keyColumn: "Id",
                keyValue: new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"),
                column: "Role",
                value: "Admin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "RoadOfGropingUsers");
        }
    }
}
