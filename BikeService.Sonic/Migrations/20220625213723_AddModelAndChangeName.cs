using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddModelAndChangeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_histories_bikes_bike_id",
                table: "bike_location_tracking_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_trackings_bikes_bike_id",
                table: "bike_location_trackings");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_station_managers_bike_stations_bike_station_id",
                table: "bike_station_managers");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_station_managers_managers_manager_id",
                table: "bike_station_managers");

            migrationBuilder.DropForeignKey(
                name: "fk_bikes_bike_stations_bike_station_id",
                table: "bikes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_managers",
                table: "managers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bikes",
                table: "bikes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_stations",
                table: "bike_stations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_station_managers",
                table: "bike_station_managers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_location_trackings",
                table: "bike_location_trackings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_location_tracking_histories",
                table: "bike_location_tracking_histories");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "managers",
                newName: "manager");

            migrationBuilder.RenameTable(
                name: "bikes",
                newName: "bike");

            migrationBuilder.RenameTable(
                name: "bike_stations",
                newName: "bike_station");

            migrationBuilder.RenameTable(
                name: "bike_station_managers",
                newName: "bike_station_manager");

            migrationBuilder.RenameTable(
                name: "bike_location_trackings",
                newName: "bike_location_tracking");

            migrationBuilder.RenameTable(
                name: "bike_location_tracking_histories",
                newName: "bike_location_tracking_history");

            migrationBuilder.RenameIndex(
                name: "ix_bikes_bike_station_id",
                table: "bike",
                newName: "ix_bike_bike_station_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_station_managers_manager_id",
                table: "bike_station_manager",
                newName: "ix_bike_station_manager_manager_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_station_managers_bike_station_id",
                table: "bike_station_manager",
                newName: "ix_bike_station_manager_bike_station_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_location_trackings_bike_id",
                table: "bike_location_tracking",
                newName: "ix_bike_location_tracking_bike_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_location_tracking_histories_bike_id",
                table: "bike_location_tracking_history",
                newName: "ix_bike_location_tracking_history_bike_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_manager",
                table: "manager",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike",
                table: "bike",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_station",
                table: "bike_station",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_station_manager",
                table: "bike_station_manager",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_location_tracking",
                table: "bike_location_tracking",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_location_tracking_history",
                table: "bike_location_tracking_history",
                column: "id");

            migrationBuilder.CreateTable(
                name: "bike_rental_booking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    checkin_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkout_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    bike_id = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "ix_bike_rental_booking_account_id",
                table: "bike_rental_booking",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_rental_booking_bike_id",
                table: "bike_rental_booking",
                column: "bike_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_bike_station_bike_station_id",
                table: "bike",
                column: "bike_station_id",
                principalTable: "bike_station",
                principalColumn: "id");

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
                name: "fk_bike_station_manager_bike_station_bike_station_id",
                table: "bike_station_manager",
                column: "bike_station_id",
                principalTable: "bike_station",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_station_manager_manager_manager_id",
                table: "bike_station_manager",
                column: "manager_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_bike_station_bike_station_id",
                table: "bike");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_bike_bike_id",
                table: "bike_location_tracking");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_location_tracking_history_bike_bike_id",
                table: "bike_location_tracking_history");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_station_manager_bike_station_bike_station_id",
                table: "bike_station_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_bike_station_manager_manager_manager_id",
                table: "bike_station_manager");

            migrationBuilder.DropTable(
                name: "bike_rental_booking");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_manager",
                table: "manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_station_manager",
                table: "bike_station_manager");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_station",
                table: "bike_station");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_location_tracking_history",
                table: "bike_location_tracking_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike_location_tracking",
                table: "bike_location_tracking");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bike",
                table: "bike");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "manager",
                newName: "managers");

            migrationBuilder.RenameTable(
                name: "bike_station_manager",
                newName: "bike_station_managers");

            migrationBuilder.RenameTable(
                name: "bike_station",
                newName: "bike_stations");

            migrationBuilder.RenameTable(
                name: "bike_location_tracking_history",
                newName: "bike_location_tracking_histories");

            migrationBuilder.RenameTable(
                name: "bike_location_tracking",
                newName: "bike_location_trackings");

            migrationBuilder.RenameTable(
                name: "bike",
                newName: "bikes");

            migrationBuilder.RenameIndex(
                name: "ix_bike_station_manager_manager_id",
                table: "bike_station_managers",
                newName: "ix_bike_station_managers_manager_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_station_manager_bike_station_id",
                table: "bike_station_managers",
                newName: "ix_bike_station_managers_bike_station_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_location_tracking_history_bike_id",
                table: "bike_location_tracking_histories",
                newName: "ix_bike_location_tracking_histories_bike_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_location_tracking_bike_id",
                table: "bike_location_trackings",
                newName: "ix_bike_location_trackings_bike_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_bike_station_id",
                table: "bikes",
                newName: "ix_bikes_bike_station_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_managers",
                table: "managers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_station_managers",
                table: "bike_station_managers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_stations",
                table: "bike_stations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_location_tracking_histories",
                table: "bike_location_tracking_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bike_location_trackings",
                table: "bike_location_trackings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bikes",
                table: "bikes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_tracking_histories_bikes_bike_id",
                table: "bike_location_tracking_histories",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_location_trackings_bikes_bike_id",
                table: "bike_location_trackings",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_station_managers_bike_stations_bike_station_id",
                table: "bike_station_managers",
                column: "bike_station_id",
                principalTable: "bike_stations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_station_managers_managers_manager_id",
                table: "bike_station_managers",
                column: "manager_id",
                principalTable: "managers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bikes_bike_stations_bike_station_id",
                table: "bikes",
                column: "bike_station_id",
                principalTable: "bike_stations",
                principalColumn: "id");
        }
    }
}
