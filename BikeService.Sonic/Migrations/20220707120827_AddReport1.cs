using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BikeService.Sonic.Migrations
{
    public partial class AddReport1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bike_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bike_id = table.Column<int>(type: "integer", nullable: false),
                    manager_id = table.Column<int>(type: "integer", nullable: false),
                    completed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    report_description = table.Column<string>(type: "text", nullable: false),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bike_report", x => x.id);
                    table.ForeignKey(
                        name: "fk_bike_report_bike_bike_id",
                        column: x => x.bike_id,
                        principalTable: "bike",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bike_report_manager_manager_id",
                        column: x => x.manager_id,
                        principalTable: "manager",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bike_report_user_account_id",
                        column: x => x.account_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bike_report_account_id",
                table: "bike_report",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_report_bike_id",
                table: "bike_report",
                column: "bike_id");

            migrationBuilder.CreateIndex(
                name: "ix_bike_report_manager_id",
                table: "bike_report",
                column: "manager_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bike_report");
        }
    }
}
