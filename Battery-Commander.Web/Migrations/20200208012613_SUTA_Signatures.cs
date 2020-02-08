using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class SUTA_Signatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommanderSignature",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CommanderSignedAt",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSergeantSignature",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstSergeantSignedAt",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "SUTAs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorSignature",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupervisorSignedAt",
                table: "SUTAs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SUTAs_SupervisorId",
                table: "SUTAs",
                column: "SupervisorId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SUTAs_Soldiers_SupervisorId",
            //    table: "SUTAs",
            //    column: "SupervisorId",
            //    principalTable: "Soldiers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SUTAs_Soldiers_SupervisorId",
                table: "SUTAs");

            migrationBuilder.DropIndex(
                name: "IX_SUTAs_SupervisorId",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "CommanderSignature",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "CommanderSignedAt",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "FirstSergeantSignature",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "FirstSergeantSignedAt",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "SupervisorSignature",
                table: "SUTAs");

            migrationBuilder.DropColumn(
                name: "SupervisorSignedAt",
                table: "SUTAs");
        }
    }
}
