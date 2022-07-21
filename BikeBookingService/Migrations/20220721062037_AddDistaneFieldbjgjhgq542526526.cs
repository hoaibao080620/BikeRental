using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class AddDistaneFieldbjgjhgq542526526 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkin_bike_station_id",
                table: "bike_rental_booking");

            migrationBuilder.DropColumn(
                name: "checkout_bike_station_id",
                table: "bike_rental_booking");

            migrationBuilder.AddColumn<string>(
                name: "checkin_bike_station",
                table: "bike_rental_booking",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "checkout_bike_station",
                table: "bike_rental_booking",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkin_bike_station",
                table: "bike_rental_booking");

            migrationBuilder.DropColumn(
                name: "checkout_bike_station",
                table: "bike_rental_booking");

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
    }
}
