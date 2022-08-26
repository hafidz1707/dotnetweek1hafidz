using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class CircleCheckDBFinal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bahan_bakar",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "ban_cadangan",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "barang_berharga",
                table: "InteriorViews");

            migrationBuilder.RenameColumn(
                name: "barang_berharga_notes",
                table: "InteriorViews",
                newName: "other_stuff_notes");

            migrationBuilder.AlterColumn<int>(
                name: "stnk",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "service_booklet",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "safety_kit",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fuel_gauge",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "other_stuff",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "spare_tire",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fuel_gauge",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "other_stuff",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "spare_tire",
                table: "InteriorViews");

            migrationBuilder.RenameColumn(
                name: "other_stuff_notes",
                table: "InteriorViews",
                newName: "barang_berharga_notes");

            migrationBuilder.AlterColumn<string>(
                name: "stnk",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "service_booklet",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "safety_kit",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "bahan_bakar",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ban_cadangan",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "barang_berharga",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);
        }
    }
}
