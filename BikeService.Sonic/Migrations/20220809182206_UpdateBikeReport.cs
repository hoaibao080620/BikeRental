using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateBikeReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "report_type",
                table: "bike_report",
                newName: "title");

            migrationBuilder.AddColumn<string>(
                name: "account_email",
                table: "bike_report",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_email",
                table: "bike_report");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "bike_report",
                newName: "report_type");
        }
    }
}
