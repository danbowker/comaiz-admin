using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddContractRateToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractRateId",
                table: "Task",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_ContractRateId",
                table: "Task",
                column: "ContractRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_ContractRate_ContractRateId",
                table: "Task",
                column: "ContractRateId",
                principalTable: "ContractRate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_ContractRate_ContractRateId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ContractRateId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ContractRateId",
                table: "Task");
        }
    }
}
