using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class ChangeTrackingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "external_id",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "external_id",
                table: "manager",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "bike_rental_tracking_id",
                table: "bike_location_tracking_history",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_bike_location_tracking_history_bike_rental_tracking_id",
                table: "bike_location_tracking_history",
                column: "bike_rental_tracking_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_history_bike_rental_booking_bike_ren",
                table: "bike_location_tracking_history",
                column: "bike_rental_tracking_id",
                principalTable: "bike_rental_booking",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_history_bike_rental_booking_bike_ren",
                table: "bike_location_tracking_history");

            migrationBuilder.DropIndex(
                name: "ix_bike_location_tracking_history_bike_rental_tracking_id",
                table: "bike_location_tracking_history");

            migrationBuilder.DropColumn(
                name: "bike_rental_tracking_id",
                table: "bike_location_tracking_history");

            migrationBuilder.AlterColumn<int>(
                name: "external_id",
                table: "user",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "external_id",
                table: "manager",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
