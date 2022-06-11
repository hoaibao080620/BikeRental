using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateBikeStationIdToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bikes_BikeStations_BikeStationId",
                table: "Bikes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BikeStationId",
                table: "Bikes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bikes_BikeStations_BikeStationId",
                table: "Bikes",
                column: "BikeStationId",
                principalTable: "BikeStations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bikes_BikeStations_BikeStationId",
                table: "Bikes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BikeStationId",
                table: "Bikes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bikes_BikeStations_BikeStationId",
                table: "Bikes",
                column: "BikeStationId",
                principalTable: "BikeStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
