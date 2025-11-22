using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskContractRateToReferenceUserContractRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskContractRate_ContractRate_ContractRateId",
                table: "TaskContractRate");

            // Add temporary column for migration
            migrationBuilder.AddColumn<int>(
                name: "UserContractRateId",
                table: "TaskContractRate",
                type: "integer",
                nullable: true);

            // Migrate data: For each TaskContractRate, find the first UserContractRate that matches the ContractRateId
            // This assumes there is at least one UserContractRate for each ContractRate
            migrationBuilder.Sql(@"
                UPDATE ""TaskContractRate"" tcr
                SET ""UserContractRateId"" = ucr.""Id""
                FROM (
                    SELECT DISTINCT ON (""ContractRateId"") ""Id"", ""ContractRateId""
                    FROM ""UserContractRate""
                    ORDER BY ""ContractRateId"", ""Id""
                ) ucr
                WHERE tcr.""ContractRateId"" = ucr.""ContractRateId"";
            ");

            // Drop the old column
            migrationBuilder.DropIndex(
                name: "IX_TaskContractRate_ContractRateId",
                table: "TaskContractRate");

            migrationBuilder.DropColumn(
                name: "ContractRateId",
                table: "TaskContractRate");

            // Make UserContractRateId non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "UserContractRateId",
                table: "TaskContractRate",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Create index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_TaskContractRate_UserContractRateId",
                table: "TaskContractRate",
                column: "UserContractRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskContractRate_UserContractRate_UserContractRateId",
                table: "TaskContractRate",
                column: "UserContractRateId",
                principalTable: "UserContractRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskContractRate_UserContractRate_UserContractRateId",
                table: "TaskContractRate");

            // Add temporary column for rollback
            migrationBuilder.AddColumn<int>(
                name: "ContractRateId",
                table: "TaskContractRate",
                type: "integer",
                nullable: true);

            // Migrate data back: Get the ContractRateId from the UserContractRate
            migrationBuilder.Sql(@"
                UPDATE ""TaskContractRate"" tcr
                SET ""ContractRateId"" = ucr.""ContractRateId""
                FROM ""UserContractRate"" ucr
                WHERE tcr.""UserContractRateId"" = ucr.""Id"";
            ");

            // Drop the UserContractRateId column
            migrationBuilder.DropIndex(
                name: "IX_TaskContractRate_UserContractRateId",
                table: "TaskContractRate");

            migrationBuilder.DropColumn(
                name: "UserContractRateId",
                table: "TaskContractRate");

            // Make ContractRateId non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "ContractRateId",
                table: "TaskContractRate",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Create index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_TaskContractRate_ContractRateId",
                table: "TaskContractRate",
                column: "ContractRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskContractRate_ContractRate_ContractRateId",
                table: "TaskContractRate",
                column: "ContractRateId",
                principalTable: "ContractRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
