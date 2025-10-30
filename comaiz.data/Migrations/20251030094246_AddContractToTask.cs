using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddContractToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Task",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_ContractId",
                table: "Task",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Contract_ContractId",
                table: "Task",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Contract_ContractId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ContractId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Task");
        }
    }
}
