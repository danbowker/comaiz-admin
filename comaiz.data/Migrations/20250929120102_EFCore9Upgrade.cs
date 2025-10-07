using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comaiz.data.Migrations
{
    /// <inheritdoc />
    public partial class EFCore9Upgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // migrationBuilder.DropColumn(
            //     name: "Discriminator",
            //     table: "WorkRecord"); // Commented out: column may not exist


            // migrationBuilder.DropColumn(
            //     name: "Discriminator",
            //     table: "FixedCost"); // Commented out: column may not exist

            // migrationBuilder.DropColumn(
            //     name: "Discriminator",
            //     table: "CarJourney");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "WorkRecord",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "FixedCost",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CarJourney",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
