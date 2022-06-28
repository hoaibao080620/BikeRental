using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class ChangeModelOfBikeTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_code",
                table: "user");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "bike_location_tracking_history",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "bike_location_tracking",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "bike_location_tracking_history");

            migrationBuilder.DropColumn(
                name: "address",
                table: "bike_location_tracking");

            migrationBuilder.AddColumn<Guid>(
                name: "account_code",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
