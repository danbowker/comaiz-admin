using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceTaskEntityRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkRecord_ContractRate_ContractRateId",
                table: "WorkRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRecord_Contract_ContractId",
                table: "WorkRecord");

            migrationBuilder.DropIndex(
                name: "IX_WorkRecord_ContractId",
                table: "WorkRecord");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "WorkRecord");

            migrationBuilder.DropColumn(
                name: "CostId",
                table: "InvoiceItem");

            migrationBuilder.RenameColumn(
                name: "ContractRateId",
                table: "WorkRecord",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkRecord_ContractRateId",
                table: "WorkRecord",
                newName: "IX_WorkRecord_TaskId");

            migrationBuilder.AddColumn<int>(
                name: "FixedCostId",
                table: "InvoiceItem",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "InvoiceItem",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_FixedCostId",
                table: "InvoiceItem",
                column: "FixedCostId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_TaskId",
                table: "InvoiceItem",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItem_FixedCost_FixedCostId",
                table: "InvoiceItem",
                column: "FixedCostId",
                principalTable: "FixedCost",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItem_Task_TaskId",
                table: "InvoiceItem",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRecord_Task_TaskId",
                table: "WorkRecord",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItem_FixedCost_FixedCostId",
                table: "InvoiceItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItem_Task_TaskId",
                table: "InvoiceItem");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRecord_Task_TaskId",
                table: "WorkRecord");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItem_FixedCostId",
                table: "InvoiceItem");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItem_TaskId",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "FixedCostId",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "InvoiceItem");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "WorkRecord",
                newName: "ContractRateId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkRecord_TaskId",
                table: "WorkRecord",
                newName: "IX_WorkRecord_ContractRateId");

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "WorkRecord",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CostId",
                table: "InvoiceItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkRecord_ContractId",
                table: "WorkRecord",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRecord_ContractRate_ContractRateId",
                table: "WorkRecord",
                column: "ContractRateId",
                principalTable: "ContractRate",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRecord_Contract_ContractId",
                table: "WorkRecord",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
