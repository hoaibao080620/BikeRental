using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddChangesToModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndLatitude",
                table: "BikeRentalTrackings");

            migrationBuilder.DropColumn(
                name: "EndLongitude",
                table: "BikeRentalTrackings");

            migrationBuilder.DropColumn(
                name: "EndedOn",
                table: "BikeRentalTrackings");

            migrationBuilder.RenameColumn(
                name: "StartedOn",
                table: "BikeRentalTrackings",
                newName: "CheckoutTime");

            migrationBuilder.RenameColumn(
                name: "StartLongitude",
                table: "BikeRentalTrackings",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "StartLatitude",
                table: "BikeRentalTrackings",
                newName: "Longitude");

            migrationBuilder.AddColumn<double>(
                name: "Point",
                table: "Users",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperManager",
                table: "Managers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckinTime",
                table: "BikeRentalTrackings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "BikeRentalTrackings",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSuperManager",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "CheckinTime",
                table: "BikeRentalTrackings");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "BikeRentalTrackings");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "BikeRentalTrackings",
                newName: "StartLongitude");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "BikeRentalTrackings",
                newName: "StartLatitude");

            migrationBuilder.RenameColumn(
                name: "CheckoutTime",
                table: "BikeRentalTrackings",
                newName: "StartedOn");

            migrationBuilder.AddColumn<double>(
                name: "EndLatitude",
                table: "BikeRentalTrackings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EndLongitude",
                table: "BikeRentalTrackings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedOn",
                table: "BikeRentalTrackings",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
