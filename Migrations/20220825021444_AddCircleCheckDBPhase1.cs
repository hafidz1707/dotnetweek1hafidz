using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class AddCircleCheckDBPhase1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "create_time",
                table: "ServiceLists",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "booking_date_time",
                table: "ServiceLists",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CircleChecks",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    service_registration_id = table.Column<int>(type: "INTEGER", nullable: false),
                    service_advisor = table.Column<string>(type: "TEXT", nullable: true),
                    service_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    dealer_service = table.Column<string>(type: "TEXT", nullable: true),
                    customer_name = table.Column<string>(type: "TEXT", nullable: true),
                    phone = table.Column<string>(type: "TEXT", nullable: true),
                    vin = table.Column<string>(type: "TEXT", nullable: true),
                    plate_number = table.Column<string>(type: "TEXT", nullable: true),
                    vehicle_model = table.Column<string>(type: "TEXT", nullable: true),
                    signature = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleChecks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ExteriorViews",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    circle_check_header_id = table.Column<int>(type: "INTEGER", nullable: false),
                    service_registration_id = table.Column<int>(type: "INTEGER", nullable: false),
                    data_type = table.Column<int>(type: "INTEGER", nullable: false),
                    data_type_text = table.Column<string>(type: "TEXT", nullable: true),
                    vehicle_condition = table.Column<int>(type: "INTEGER", nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    image_path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExteriorViews", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CircleChecks");

            migrationBuilder.DropTable(
                name: "ExteriorViews");

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_time",
                table: "ServiceLists",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "booking_date_time",
                table: "ServiceLists",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
