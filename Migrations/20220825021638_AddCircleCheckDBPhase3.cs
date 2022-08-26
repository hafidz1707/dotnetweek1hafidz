using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class AddCircleCheckDBPhase3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InteriorViews",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    circle_check_header_id = table.Column<int>(type: "INTEGER", nullable: false),
                    service_registration_id = table.Column<int>(type: "INTEGER", nullable: false),
                    stnk = table.Column<string>(type: "TEXT", nullable: true),
                    service_booklet = table.Column<string>(type: "TEXT", nullable: true),
                    ban_cadangan = table.Column<string>(type: "TEXT", nullable: true),
                    safety_kit = table.Column<string>(type: "TEXT", nullable: true),
                    bahan_bakar = table.Column<string>(type: "TEXT", nullable: true),
                    barang_berharga = table.Column<string>(type: "TEXT", nullable: true),
                    barang_berharga_notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InteriorViews");
        }
    }
}
