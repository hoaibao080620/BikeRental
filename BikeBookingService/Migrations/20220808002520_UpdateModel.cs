using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class UpdateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_bikes_bike_id",
                table: "bike_location_tracking");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_history_bikes_bike_id",
                table: "bike_location_tracking_history");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_rental_booking_bikes_bike_id",
                table: "bike_rental_booking");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bikes",
                table: "bikes");

            migrationBuilder.RenameTable(
                name: "bikes",
                newName: "bike");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike",
                table: "bike",
                column: "id");

            migrationBuilder.CreateTable(
                name: "renting_point",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    point_per_hour = table.Column<double>(type: "double precision", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renting_point", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "renting_point_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    point_change = table.Column<double>(type: "double precision", nullable: false),
                    change_reason = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    change_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renting_point_history", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_bike_bike_id",
                table: "bike_location_tracking",
                column: "bike_id",
                principalTable: "bike",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_history_bike_bike_id",
                table: "bike_location_tracking_history",
                column: "bike_id",
                principalTable: "bike",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_rental_booking_bike_bike_id",
                table: "bike_rental_booking",
                column: "bike_id",
                principalTable: "bike",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_bike_bike_id",
                table: "bike_location_tracking");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_history_bike_bike_id",
                table: "bike_location_tracking_history");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_rental_booking_bike_bike_id",
                table: "bike_rental_booking");

            migrationBuilder.DropTable(
                name: "renting_point");

            migrationBuilder.DropTable(
                name: "renting_point_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike",
                table: "bike");

            migrationBuilder.RenameTable(
                name: "bike",
                newName: "bikes");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bikes",
                table: "bikes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_bikes_bike_id",
                table: "bike_location_tracking",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_history_bikes_bike_id",
                table: "bike_location_tracking_history",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_rental_booking_bikes_bike_id",
                table: "bike_rental_booking",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
