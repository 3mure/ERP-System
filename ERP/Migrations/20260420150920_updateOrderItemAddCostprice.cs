using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog_Service.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderItemAddCostprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "sales_order_items",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "sales_order_items");
        }
    }
}
