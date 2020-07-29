using Microsoft.EntityFrameworkCore.Migrations;

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

            migrationBuilder.AlterColumn<decimal>(
                name: "StandingPowerThrow",
                table: "ACFTs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
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
