using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceStateAndInvoiceItemDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "InvoiceItem",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "InvoiceItem",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Invoice",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Invoice");
        }
    }
}
