using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Online.Store.Migrations
{
    public partial class order_change_productprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetail");

            migrationBuilder.AddColumn<double>(
                name: "ProductPrice",
                table: "OrderDetail",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "OrderDetail");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "OrderDetail",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
