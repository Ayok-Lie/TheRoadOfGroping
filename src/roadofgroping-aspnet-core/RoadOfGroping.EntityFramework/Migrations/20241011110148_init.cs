using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadOfGroping.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSetting",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    clientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    publicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    privateKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSetting", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", maxLength: 36, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionOriginal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "权限名称"),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "权限代码"),
                    ParentCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "父权限代码"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false, comment: "是否系统权限"),
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
                    table.PrimaryKey("PK_PermissionOriginal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRoleRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "角色Id"),
                    PermissionCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "权限编码"),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false, comment: "是否授权"),
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
                    table.PrimaryKey("PK_PermissionRoleRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "角色名称"),
                    RoleCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "角色代码"),
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
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "用户名"),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "密码哈希值"),
                    UserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "邮箱"),
                    UserPhone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: "手机号"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false, comment: "用户Id"),
                    RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false, comment: "角色Id"),
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
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleCode",
                table: "Roles",
                column: "RoleCode",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSetting");

            migrationBuilder.DropTable(
                name: "FileInfos");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "PermissionOriginal");

            migrationBuilder.DropTable(
                name: "PermissionRoleRelation");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}