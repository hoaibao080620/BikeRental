using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateBikeStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "used_parking_space",
                table: "bike_station");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "bike_station",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "bike_station");

            migrationBuilder.AddColumn<int>(
                name: "used_parking_space",
                table: "bike_station",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
