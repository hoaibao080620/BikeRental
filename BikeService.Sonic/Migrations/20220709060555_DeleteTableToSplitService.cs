using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class DeleteTableToSplitService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_user_account_id",
                table: "bike_report");

            migrationBuilder.DropTable(
                name: "bike_location_tracking");

            migrationBuilder.DropTable(
                name: "bike_location_tracking_history");

            migrationBuilder.DropTable(
                name: "bike_rental_booking");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropIndex(
                name: "ix_bike_report_account_id",
                table: "bike_report");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "bike_report");

            migrationBuilder.AddColumn<string>(
                name: "account_email",
                table: "bike_report",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_email",
                table: "bike_report");

            migrationBuilder.AddColumn<int>(
                name: "account_id",
                table: "bike_report",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "bike_location_tracking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bike_id = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_location_tracking", x => x.id);
                    table.ForeignKey(
                        name: "fk_bike_location_tracking_bike_bike_id",
                        column: x => x.bike_id,
                        principalTable: "bike",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    point = table.Column<double>(type: "double precision", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bike_rental_booking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    bike_id = table.Column<int>(type: "integer", nullable: false),
                    checkin_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkout_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    total_point = table.Column<double>(type: "double precision", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_rental_booking", x => x.id);
                    table.ForeignKey(
                        name: "fk_bike_rental_booking_bike_bike_id",
                        column: x => x.bike_id,
                        principalTable: "bike",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bike_rental_booking_user_account_id",
                        column: x => x.account_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bike_location_tracking_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bike_id = table.Column<int>(type: "integer", nullable: false),
                    bike_rental_tracking_id = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_location_tracking_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_bike_location_tracking_history_bike_bike_id",
                        column: x => x.bike_id,
                        principalTable: "bike",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bike_location_tracking_history_bike_rental_booking_bike_ren",
                        column: x => x.bike_rental_tracking_id,
                        principalTable: "bike_rental_booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bike_report_account_id",
                table: "bike_report",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_location_tracking_bike_id",
                table: "bike_location_tracking",
                column: "bike_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_location_tracking_history_bike_id",
                table: "bike_location_tracking_history",
                column: "bike_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_location_tracking_history_bike_rental_tracking_id",
                table: "bike_location_tracking_history",
                column: "bike_rental_tracking_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_rental_booking_account_id",
                table: "bike_rental_booking",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_rental_booking_bike_id",
                table: "bike_rental_booking",
                column: "bike_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_user_account_id",
                table: "bike_report",
                column: "account_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
