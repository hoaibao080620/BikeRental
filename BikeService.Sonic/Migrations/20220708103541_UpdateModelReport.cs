using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class UpdateModelReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_manager_manager_id",
                table: "bike_report");

            migrationBuilder.RenameColumn(
                name: "manager_id",
                table: "bike_report",
                newName: "assign_to_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_report_manager_id",
                table: "bike_report",
                newName: "ix_bike_report_assign_to_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report",
                column: "assign_to_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_manager_assign_to_id",
                table: "bike_report");

            migrationBuilder.RenameColumn(
                name: "assign_to_id",
                table: "bike_report",
                newName: "manager_id");

            migrationBuilder.RenameIndex(
                name: "ix_bike_report_assign_to_id",
                table: "bike_report",
                newName: "ix_bike_report_manager_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_manager_manager_id",
                table: "bike_report",
                column: "manager_id",
                principalTable: "manager",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
