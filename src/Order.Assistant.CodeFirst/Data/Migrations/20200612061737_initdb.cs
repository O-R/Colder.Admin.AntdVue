using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Order.Assistant.CodeFirst.Data.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "x_customer",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(maxLength: 50, nullable: true),
                    CreatorRealName = table.Column<string>(maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    CustomerNo = table.Column<string>(maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_x_customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "x_sku",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(maxLength: 50, nullable: true),
                    CreatorRealName = table.Column<string>(maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    SkuNo = table.Column<string>(maxLength: 50, nullable: true),
                    SkuName = table.Column<string>(maxLength: 100, nullable: true),
                    KeyWords = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_x_sku", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "x_order",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(maxLength: 50, nullable: true),
                    CreatorRealName = table.Column<string>(maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    OrderNo = table.Column<string>(maxLength: 50, nullable: true),
                    CustomerNo = table.Column<string>(maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(maxLength: 50, nullable: true),
                    CustomerId = table.Column<string>(maxLength: 50, nullable: true),
                    ProvinceNo = table.Column<string>(maxLength: 50, nullable: true),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    CityNo = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    AreaNo = table.Column<string>(maxLength: 50, nullable: true),
                    Area = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    Receiver = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiverPhone = table.Column<string>(maxLength: 50, nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_x_order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_x_order_x_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "x_customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "x_customerSku",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(maxLength: 50, nullable: true),
                    CreatorRealName = table.Column<string>(maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 50, nullable: true),
                    SkuId = table.Column<string>(maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_x_customerSku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_x_customerSku_x_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "x_customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_x_customerSku_x_sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "x_sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "x_orderItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(maxLength: 50, nullable: true),
                    CreatorRealName = table.Column<string>(maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    OrderId = table.Column<string>(maxLength: 50, nullable: true),
                    SkuNo = table.Column<string>(maxLength: 50, nullable: true),
                    SkuId = table.Column<string>(maxLength: 50, nullable: true),
                    SkuName = table.Column<string>(maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_x_orderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_x_orderItem_x_order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "x_order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_x_orderItem_x_sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "x_sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_x_customerSku_CustomerId",
                table: "x_customerSku",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_x_customerSku_SkuId",
                table: "x_customerSku",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_x_order_CustomerId",
                table: "x_order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_x_orderItem_OrderId",
                table: "x_orderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_x_orderItem_SkuId",
                table: "x_orderItem",
                column: "SkuId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "x_customerSku");

            migrationBuilder.DropTable(
                name: "x_orderItem");

            migrationBuilder.DropTable(
                name: "x_order");

            migrationBuilder.DropTable(
                name: "x_sku");

            migrationBuilder.DropTable(
                name: "x_customer");
        }
    }
}
