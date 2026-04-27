using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfrawyStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveMinimumStockToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumStock",
                table: "Inventories");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumStock",
                table: "Products",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumStock",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumStock",
                table: "Inventories",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 5m);
        }
    }
}
