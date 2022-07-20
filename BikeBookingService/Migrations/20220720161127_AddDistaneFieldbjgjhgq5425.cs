using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class AddDistaneFieldbjgjhgq5425 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "checkin_bike_station_id",
                table: "bike_rental_booking",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "checkout_bike_station_id",
                table: "bike_rental_booking",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkin_bike_station_id",
                table: "bike_rental_booking");

            migrationBuilder.DropColumn(
                name: "checkout_bike_station_id",
                table: "bike_rental_booking");
        }
    }
}
