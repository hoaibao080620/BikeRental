using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateBikeReport1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report");

            migrationBuilder.AlterColumn<int>(
                name: "assign_to_id",
                table: "bike_report",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report",
                column: "assign_to_id",
                principalTable: "manager",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report");

            migrationBuilder.AlterColumn<int>(
                name: "assign_to_id",
                table: "bike_report",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report",
                column: "assign_to_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
