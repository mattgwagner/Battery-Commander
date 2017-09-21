using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class VehicleModelTweaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles");

            migrationBuilder.AddColumn<string>(
                name: "LIN",
                table: "Vehicles",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Registration",
                table: "Vehicles",
                column: "Registration",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Serial",
                table: "Vehicles",
                column: "Serial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UnitId_Bumper",
                table: "Vehicles",
                columns: new[] { "UnitId", "Bumper" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Registration",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Serial",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UnitId_Bumper",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LIN",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles",
                column: "UnitId");
        }
    }
}
