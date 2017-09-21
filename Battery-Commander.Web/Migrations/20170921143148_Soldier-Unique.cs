using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class SoldierUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Units_UIC",
                table: "Units",
                column: "UIC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soldiers_DoDId",
                table: "Soldiers",
                column: "DoDId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soldiers_FirstName_MiddleName_LastName",
                table: "Soldiers",
                columns: new[] { "FirstName", "MiddleName", "LastName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Units_UIC",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Soldiers_DoDId",
                table: "Soldiers");

            migrationBuilder.DropIndex(
                name: "IX_Soldiers_FirstName_MiddleName_LastName",
                table: "Soldiers");
        }
    }
}
