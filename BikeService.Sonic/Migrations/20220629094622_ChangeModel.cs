using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class ChangeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount",
                table: "bike_rental_booking",
                newName: "total_point");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total_point",
                table: "bike_rental_booking",
                newName: "amount");
        }
    }
}
