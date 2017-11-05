using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Online.Store.Migrations
{
    public partial class order_add_product_title_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductModel",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductTitle",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductModel",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ProductTitle",
                table: "OrderDetail");
        }
    }
}
