using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddManyToOneModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BikeStationManagerId",
                table: "BikeStationManagers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BikeStationManagers_BikeStationManagerId",
                table: "BikeStationManagers",
                column: "BikeStationManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BikeStationManagers_BikeStationManagers_BikeStationManagerId",
                table: "BikeStationManagers",
                column: "BikeStationManagerId",
                principalTable: "BikeStationManagers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BikeStationManagers_BikeStationManagers_BikeStationManagerId",
                table: "BikeStationManagers");

            migrationBuilder.DropIndex(
                name: "IX_BikeStationManagers_BikeStationManagerId",
                table: "BikeStationManagers");

            migrationBuilder.DropColumn(
                name: "BikeStationManagerId",
                table: "BikeStationManagers");
        }
    }
}
