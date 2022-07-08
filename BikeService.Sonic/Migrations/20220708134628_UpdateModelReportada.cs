using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateModelReportada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_resolved",
                table: "bike_report");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "bike_report",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "bike_report");

            migrationBuilder.AddColumn<bool>(
                name: "is_resolved",
                table: "bike_report",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
