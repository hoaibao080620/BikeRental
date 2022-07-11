using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class DropStatusColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "bikes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "bikes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
