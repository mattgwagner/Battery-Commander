using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Migrations
{
    public partial class VehicleChalk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Chalk",
                table: "Vehicles",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "OrderOfMarch",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chalk",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "OrderOfMarch",
                table: "Vehicles");
        }
    }
}
