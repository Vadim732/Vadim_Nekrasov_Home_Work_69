using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Migrations
{
    /// <inheritdoc />
    public partial class addedAssignmentToUserPerformrAndUserCreatorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserCreatorId",
                table: "Assignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserPerformerId",
                table: "Assignments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_UserCreatorId",
                table: "Assignments",
                column: "UserCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_UserPerformerId",
                table: "Assignments",
                column: "UserPerformerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AspNetUsers_UserCreatorId",
                table: "Assignments",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AspNetUsers_UserPerformerId",
                table: "Assignments",
                column: "UserPerformerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AspNetUsers_UserCreatorId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AspNetUsers_UserPerformerId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_UserCreatorId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_UserPerformerId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "UserPerformerId",
                table: "Assignments");
        }
    }
}
