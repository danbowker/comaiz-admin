using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorContractRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "InvoiceDescription",
                table: "ContractRate",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserContractRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractRateId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContractRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContractRate_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserContractRate_ContractRate_ContractRateId",
                        column: x => x.ContractRateId,
                        principalTable: "ContractRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserContractRate_ApplicationUserId",
                table: "UserContractRate",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContractRate_ContractRateId",
                table: "UserContractRate",
                column: "ContractRateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserContractRate");

            migrationBuilder.DropColumn(
                name: "InvoiceDescription",
                table: "ContractRate");

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
    }
}
