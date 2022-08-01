using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class hhhhh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bike_report_bike_report_type_report_type_id",
                table: "bike_report");

            migrationBuilder.DropTable(
                name: "bike_report_type");

            migrationBuilder.DropIndex(
                name: "ix_bike_report_report_type_id",
                table: "bike_report");

            migrationBuilder.DropColumn(
                name: "report_type_id",
                table: "bike_report");

            migrationBuilder.RenameColumn(
                name: "account_email",
                table: "bike_report",
                newName: "report_type");

            migrationBuilder.AddColumn<string>(
                name: "account_phone_number",
                table: "bike_report",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "bike_report",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_phone_number",
                table: "bike_report");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "bike_report");

            migrationBuilder.RenameColumn(
                name: "report_type",
                table: "bike_report",
                newName: "account_email");

            migrationBuilder.AddColumn<int>(
                name: "report_type_id",
                table: "bike_report",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "bike_report_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_report_type", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bike_report_report_type_id",
                table: "bike_report",
                column: "report_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bike_report_bike_report_type_report_type_id",
                table: "bike_report",
                column: "report_type_id",
                principalTable: "bike_report_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
