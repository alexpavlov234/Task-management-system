using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_management_system.Migrations
{
    public partial class InititalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "User",
                newName: "UserRoleRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_User_RoleId",
                table: "User",
                newName: "IX_User_UserRoleRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_UserRoleRoleId",
                table: "User",
                column: "UserRoleRoleId",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_UserRoleRoleId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Username",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "UserRoleRoleId",
                table: "User",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserRoleRoleId",
                table: "User",
                newName: "IX_User_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
