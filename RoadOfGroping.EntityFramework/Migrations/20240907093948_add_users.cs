using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadOfGroping.EntityFramework.Migrations
{
    public partial class add_users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadOfGropingUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(64)", maxLength: 32, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<string>(type: "nvarchar(64)", maxLength: 32, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    ModifierId = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadOfGropingUsers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RoadOfGropingUsers",
                columns: new[] { "Id", "Avatar", "CreationTime", "CreatorId", "DeleterId", "DeletionTime", "IsDeleted", "ModificationTime", "ModifierId", "PasswordHash", "UserEmail", "UserName", "UserPhone" },
                values: new object[] { new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"), null, null, null, null, null, null, null, null, "bb123456", "admin@localhost", "admin", "8888888888" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoadOfGropingUsers");
        }
    }
}
