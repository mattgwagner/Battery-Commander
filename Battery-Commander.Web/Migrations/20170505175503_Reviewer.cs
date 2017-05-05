using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class Reviewer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewerId",
                table: "Evaluations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ReviewerId",
                table: "Evaluations",
                column: "ReviewerId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Evaluations_Soldiers_ReviewerId",
            //    table: "Evaluations",
            //    column: "ReviewerId",
            //    principalTable: "Soldiers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Soldiers_ReviewerId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_ReviewerId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Evaluations");
        }
    }
}
