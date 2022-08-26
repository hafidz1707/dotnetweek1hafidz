using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class AddCircleCheckDBPhase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TireViews",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    circle_check_header_id = table.Column<int>(type: "INTEGER", nullable: false),
                    service_registration_id = table.Column<int>(type: "INTEGER", nullable: false),
                    front_right = table.Column<string>(type: "TEXT", nullable: true),
                    front_left = table.Column<string>(type: "TEXT", nullable: true),
                    back_right = table.Column<string>(type: "TEXT", nullable: true),
                    back_left = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TireViews");
        }
    }
}
