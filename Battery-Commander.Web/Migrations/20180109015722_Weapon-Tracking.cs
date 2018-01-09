using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Migrations
{
    public partial class WeaponTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdminNumber = table.Column<string>(maxLength: 10, nullable: false),
                    AssignedId = table.Column<int>(nullable: true),
                    OpticSerial = table.Column<string>(maxLength: 50, nullable: true),
                    OpticType = table.Column<byte>(nullable: false),
                    Serial = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    UnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapons_Soldiers_AssignedId",
                        column: x => x.AssignedId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Weapons_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_AssignedId",
                table: "Weapons",
                column: "AssignedId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_UnitId",
                table: "Weapons",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_Serial_Type",
                table: "Weapons",
                columns: new[] { "Serial", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weapons");
        }
    }
}
