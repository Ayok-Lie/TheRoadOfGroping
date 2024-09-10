using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadOfGroping.EntityFramework.Migrations
{
    public partial class add_FileInfos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModifierId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "RoadOfGropingUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleterId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FileInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    FileDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FileTypeString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInfos", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "RoadOfGropingUsers",
                keyColumn: "Id",
                keyValue: new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"),
                column: "IsDeleted",
                value: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileInfos");

            migrationBuilder.AlterColumn<string>(
                name: "ModifierId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "RoadOfGropingUsers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DeleterId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "RoadOfGropingUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "RoadOfGropingUsers",
                keyColumn: "Id",
                keyValue: new Guid("45d6422e-0ebb-45db-dc2a-08dc86a36122"),
                column: "IsDeleted",
                value: null);
        }
    }
}
