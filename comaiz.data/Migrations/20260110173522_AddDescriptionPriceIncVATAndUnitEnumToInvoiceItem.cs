using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionPriceIncVATAndUnitEnumToInvoiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InvoiceItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceIncVAT",
                table: "InvoiceItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InvoiceItem",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "PriceIncVAT",
                table: "InvoiceItem");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InvoiceItem",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
