using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserToWorkRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "WorkRecord",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkRecord_ApplicationUserId",
                table: "WorkRecord",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRecord_AspNetUsers_ApplicationUserId",
                table: "WorkRecord",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkRecord_AspNetUsers_ApplicationUserId",
                table: "WorkRecord");

            migrationBuilder.DropIndex(
                name: "IX_WorkRecord_ApplicationUserId",
                table: "WorkRecord");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WorkRecord");
        }
    }
}
