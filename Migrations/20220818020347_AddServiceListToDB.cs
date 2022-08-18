using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class AddServiceListToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceLists",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    reg_number = table.Column<string>(type: "TEXT", nullable: true),
                    queue_number = table.Column<string>(type: "TEXT", nullable: true),
                    service_advisor = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    plate_number = table.Column<string>(type: "TEXT", nullable: true),
                    input_source_id = table.Column<int>(type: "INTEGER", nullable: false),
                    input_source = table.Column<string>(type: "TEXT", nullable: true),
                    preferred_sa = table.Column<string>(type: "TEXT", nullable: true),
                    estimated_service = table.Column<string>(type: "TEXT", nullable: true),
                    waiting_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: true),
                    is_vip = table.Column<string>(type: "TEXT", nullable: true),
                    is_rework = table.Column<string>(type: "TEXT", nullable: true),
                    is_ontime = table.Column<string>(type: "TEXT", nullable: true),
                    booking_date_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    create_time = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLists", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceLists");
        }
    }
}
