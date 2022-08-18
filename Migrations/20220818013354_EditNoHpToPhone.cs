using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class EditNoHpToPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "no_hp",
                table: "UsersChanger",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "no_hp",
                table: "Users",
                newName: "phone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "phone",
                table: "UsersChanger",
                newName: "no_hp");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Users",
                newName: "no_hp");
        }
    }
}
