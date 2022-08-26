using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class CircleCheckDBFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "TireViews",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "back_left_photo_1",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "back_left_photo_2",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "back_left_photo_3",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "back_right_photo_1",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "back_right_photo_2",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "back_right_photo_3",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_left_photo_1",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_left_photo_2",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_left_photo_3",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_right_photo_1",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_right_photo_2",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "front_right_photo_3",
                table: "TireViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "interior_photo_1",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interior_photo_2",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interior_photo_3",
                table: "InteriorViews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TireViews",
                table: "TireViews",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InteriorViews",
                table: "InteriorViews",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TireViews",
                table: "TireViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InteriorViews",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "back_left_photo_1",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "back_left_photo_2",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "back_left_photo_3",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "back_right_photo_1",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "back_right_photo_2",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "back_right_photo_3",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_left_photo_1",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_left_photo_2",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_left_photo_3",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_right_photo_1",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_right_photo_2",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "front_right_photo_3",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "interior_photo_1",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "interior_photo_2",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "interior_photo_3",
                table: "InteriorViews");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "TireViews",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
