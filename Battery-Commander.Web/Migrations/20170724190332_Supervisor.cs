using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class Supervisor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Soldiers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soldiers_SupervisorId",
                table: "Soldiers",
                column: "SupervisorId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Soldiers_Soldiers_SupervisorId",
            //    table: "Soldiers",
            //    column: "SupervisorId",
            //    principalTable: "Soldiers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Soldiers_Soldiers_SupervisorId",
            //    table: "Soldiers");

            migrationBuilder.DropIndex(
                name: "IX_Soldiers_SupervisorId",
                table: "Soldiers");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Soldiers");
        }
    }
}
