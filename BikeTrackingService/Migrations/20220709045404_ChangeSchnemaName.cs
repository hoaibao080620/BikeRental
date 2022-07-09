using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeTrackingService.Migrations
{
    public partial class ChangeSchnemaName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_rental_booking_user_account_id",
                table: "bike_rental_booking");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "account");

            migrationBuilder.AddPrimaryKey(
                name: "pk_account",
                table: "account",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_rental_booking_account_account_id",
                table: "bike_rental_booking",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_rental_booking_account_account_id",
                table: "bike_rental_booking");

            migrationBuilder.DropPrimaryKey(
                name: "pk_account",
                table: "account");

            migrationBuilder.RenameTable(
                name: "account",
                newName: "user");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_rental_booking_user_account_id",
                table: "bike_rental_booking",
                column: "account_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
