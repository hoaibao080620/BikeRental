using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class AddOlumnsss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_warning_notification_send",
                table: "bike_rental_booking",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_warning_notification_send",
                table: "bike_rental_booking");
        }
    }
}
