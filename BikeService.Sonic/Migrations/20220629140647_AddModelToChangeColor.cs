using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddModelToChangeColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bike_station_color",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    color = table.Column<string>(type: "text", nullable: false),
                    bike_station_id = table.Column<int>(type: "integer", nullable: false),
                    manager_id = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_station_color", x => x.id);
                    table.ForeignKey(
                        name: "fk_bike_station_color_bike_station_bike_station_id",
                        column: x => x.bike_station_id,
                        principalTable: "bike_station",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bike_station_color_manager_manager_id",
                        column: x => x.manager_id,
                        principalTable: "manager",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bike_station_color_bike_station_id",
                table: "bike_station_color",
                column: "bike_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_station_color_manager_id",
                table: "bike_station_color",
                column: "manager_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bike_station_color");
        }
    }
}
