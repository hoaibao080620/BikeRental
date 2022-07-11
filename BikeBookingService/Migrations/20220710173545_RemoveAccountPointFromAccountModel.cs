using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBookingService.Migrations
{
    public partial class RemoveAccountPointFromAccountModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "point",
                table: "account");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "point",
                table: "account",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
