using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Soldiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CivilianEmail = table.Column<string>(maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    DoDId = table.Column<string>(maxLength: 12, nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    MilitaryEmail = table.Column<string>(maxLength: 50, nullable: true),
                    Rank = table.Column<byte>(nullable: false),
                    UnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soldiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Soldiers_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ABCPs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Height = table.Column<decimal>(nullable: false),
                    MeasurementsJson = table.Column<string>(nullable: true),
                    SoldierId = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ABCPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ABCPs_Soldiers_SoldierId",
                        column: x => x.SoldierId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APFTs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    PushUps = table.Column<int>(nullable: false),
                    RunSeconds = table.Column<int>(nullable: false),
                    SitUps = table.Column<int>(nullable: false),
                    SoldierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APFTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APFTs_Soldiers_SoldierId",
                        column: x => x.SoldierId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RateeId = table.Column<int>(nullable: false),
                    RaterId = table.Column<int>(nullable: false),
                    SeniorRaterId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    ThruDate = table.Column<DateTime>(type: "date", nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Soldiers_RateeId",
                        column: x => x.RateeId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Soldiers_RaterId",
                        column: x => x.RaterId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Soldiers_SeniorRaterId",
                        column: x => x.SeniorRaterId,
                        principalTable: "Soldiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Author = table.Column<string>(nullable: true),
                    EvaluationId = table.Column<int>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Evaluations_EvaluationId",
                        column: x => x.EvaluationId,
                        principalTable: "Evaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ABCPs_SoldierId",
                table: "ABCPs",
                column: "SoldierId");

            migrationBuilder.CreateIndex(
                name: "IX_APFTs_SoldierId",
                table: "APFTs",
                column: "SoldierId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_RateeId",
                table: "Evaluations",
                column: "RateeId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_RaterId",
                table: "Evaluations",
                column: "RaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_SeniorRaterId",
                table: "Evaluations",
                column: "SeniorRaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EvaluationId",
                table: "Event",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_Soldiers_UnitId",
                table: "Soldiers",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ABCPs");

            migrationBuilder.DropTable(
                name: "APFTs");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "Soldiers");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
