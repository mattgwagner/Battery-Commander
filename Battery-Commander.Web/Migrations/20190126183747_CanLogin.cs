using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Migrations
{
    public partial class CanLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanLogin",
                table: "Soldiers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE Soldiers SET CanLogin = 1 WHERE CivilianEmail IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanLogin",
                table: "Soldiers");
        }
    }
}
