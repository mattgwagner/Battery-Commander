using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BatteryCommander.Web.Migrations
{
    public partial class ACFT_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ACFT_Grading_Standard",
                table: "Soldiers",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.DropTable(name: "ACFTs");

            migrationBuilder.CreateTable(
                name: "ACFTs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SoldierId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    ForRecord = table.Column<bool>(nullable: false),
                    ThreeRepMaximumDeadlifts = table.Column<int>(nullable: false),
                    StandingPowerThrow = table.Column<decimal>(nullable: false),
                    HandReleasePushups = table.Column<int>(nullable: false),
                    SprintDragCarrySeconds = table.Column<int>(nullable: false),
                    LegTucks = table.Column<int>(nullable: false),
                    TwoMileRunSeconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACFTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ACFTs_Soldiers_SoldierId",
                        column: x => x.SoldierId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACFTs_SoldierId",
                table: "ACFTs",
                column: "SoldierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACFT_Grading_Standard",
                table: "Soldiers");

            migrationBuilder.AlterColumn<int>(
                name: "StandingPowerThrow",
                table: "ACFTs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
