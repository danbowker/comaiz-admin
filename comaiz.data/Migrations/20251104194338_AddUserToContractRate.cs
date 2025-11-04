using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToContractRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ContractRate",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractRate_ApplicationUserId",
                table: "ContractRate",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractRate_AspNetUsers_ApplicationUserId",
                table: "ContractRate",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractRate_AspNetUsers_ApplicationUserId",
                table: "ContractRate");

            migrationBuilder.DropIndex(
                name: "IX_ContractRate_ApplicationUserId",
                table: "ContractRate");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ContractRate");
        }
    }
}
