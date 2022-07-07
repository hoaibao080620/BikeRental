using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "user",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "user");
        }
    }
}
