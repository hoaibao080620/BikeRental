using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class teadaf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payment_status",
                table: "bike_rental_booking",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "bike_rental_booking");
        }
    }
}
