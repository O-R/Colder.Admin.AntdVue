using Microsoft.EntityFrameworkCore.Migrations;

namespace Order.Assistant.CodeFirst.Data.Migrations
{
    public partial class alter_order_orderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Sku_SkuId",
                table: "OrderItem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Sku_SkuId",
                table: "OrderItem",
                column: "SkuId",
                principalTable: "Sku",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
