using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskContractRateCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskContractRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    ContractRateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskContractRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskContractRate_ContractRate_ContractRateId",
                        column: x => x.ContractRateId,
                        principalTable: "ContractRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskContractRate_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskContractRate_ContractRateId",
                table: "TaskContractRate",
                column: "ContractRateId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskContractRate_TaskId",
                table: "TaskContractRate",
                column: "TaskId");

            // Migrate existing contract rate references to the collection
            migrationBuilder.Sql(@"
                INSERT INTO ""TaskContractRate"" (""TaskId"", ""ContractRateId"")
                SELECT ""Id"", ""ContractRateId""
                FROM ""Task""
                WHERE ""ContractRateId"" IS NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskContractRate");
        }
    }
}
