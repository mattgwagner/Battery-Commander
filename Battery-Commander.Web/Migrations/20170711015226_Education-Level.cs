using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BatteryCommander.Web.Migrations
{
    public partial class EducationLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "EducationLevel",
                table: "Soldiers",
                nullable: false,
                defaultValue: (byte)MilitaryEducationLevel.Unknown);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Soldiers");
        }
    }
}