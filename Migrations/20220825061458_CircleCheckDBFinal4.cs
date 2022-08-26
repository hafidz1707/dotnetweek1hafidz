﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekOneApi.Migrations
{
    public partial class CircleCheckDBFinal4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintViews_CircleChecks_CircleCheckid",
                table: "ComplaintViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ExteriorViews_CircleChecks_CircleCheckid",
                table: "ExteriorViews");

            migrationBuilder.DropForeignKey(
                name: "FK_InteriorViews_CircleChecks_CircleCheckid",
                table: "InteriorViews");

            migrationBuilder.DropForeignKey(
                name: "FK_TireViews_CircleChecks_CircleCheckid",
                table: "TireViews");

            migrationBuilder.DropIndex(
                name: "IX_TireViews_CircleCheckid",
                table: "TireViews");

            migrationBuilder.DropIndex(
                name: "IX_InteriorViews_CircleCheckid",
                table: "InteriorViews");

            migrationBuilder.DropIndex(
                name: "IX_ExteriorViews_CircleCheckid",
                table: "ExteriorViews");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintViews_CircleCheckid",
                table: "ComplaintViews");

            migrationBuilder.DropColumn(
                name: "CircleCheckid",
                table: "TireViews");

            migrationBuilder.DropColumn(
                name: "CircleCheckid",
                table: "InteriorViews");

            migrationBuilder.DropColumn(
                name: "CircleCheckid",
                table: "ExteriorViews");

            migrationBuilder.DropColumn(
                name: "CircleCheckid",
                table: "ComplaintViews");

            migrationBuilder.CreateIndex(
                name: "IX_TireViews_circle_check_header_id",
                table: "TireViews",
                column: "circle_check_header_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InteriorViews_circle_check_header_id",
                table: "InteriorViews",
                column: "circle_check_header_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExteriorViews_circle_check_header_id",
                table: "ExteriorViews",
                column: "circle_check_header_id");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintViews_circle_check_header_id",
                table: "ComplaintViews",
                column: "circle_check_header_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintViews_CircleChecks_circle_check_header_id",
                table: "ComplaintViews",
                column: "circle_check_header_id",
                principalTable: "CircleChecks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExteriorViews_CircleChecks_circle_check_header_id",
                table: "ExteriorViews",
                column: "circle_check_header_id",
                principalTable: "CircleChecks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InteriorViews_CircleChecks_circle_check_header_id",
                table: "InteriorViews",
                column: "circle_check_header_id",
                principalTable: "CircleChecks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TireViews_CircleChecks_circle_check_header_id",
                table: "TireViews",
                column: "circle_check_header_id",
                principalTable: "CircleChecks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintViews_CircleChecks_circle_check_header_id",
                table: "ComplaintViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ExteriorViews_CircleChecks_circle_check_header_id",
                table: "ExteriorViews");

            migrationBuilder.DropForeignKey(
                name: "FK_InteriorViews_CircleChecks_circle_check_header_id",
                table: "InteriorViews");

            migrationBuilder.DropForeignKey(
                name: "FK_TireViews_CircleChecks_circle_check_header_id",
                table: "TireViews");

            migrationBuilder.DropIndex(
                name: "IX_TireViews_circle_check_header_id",
                table: "TireViews");

            migrationBuilder.DropIndex(
                name: "IX_InteriorViews_circle_check_header_id",
                table: "InteriorViews");

            migrationBuilder.DropIndex(
                name: "IX_ExteriorViews_circle_check_header_id",
                table: "ExteriorViews");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintViews_circle_check_header_id",
                table: "ComplaintViews");

            migrationBuilder.AddColumn<int>(
                name: "CircleCheckid",
                table: "TireViews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CircleCheckid",
                table: "InteriorViews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CircleCheckid",
                table: "ExteriorViews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CircleCheckid",
                table: "ComplaintViews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TireViews_CircleCheckid",
                table: "TireViews",
                column: "CircleCheckid");

            migrationBuilder.CreateIndex(
                name: "IX_InteriorViews_CircleCheckid",
                table: "InteriorViews",
                column: "CircleCheckid");

            migrationBuilder.CreateIndex(
                name: "IX_ExteriorViews_CircleCheckid",
                table: "ExteriorViews",
                column: "CircleCheckid");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintViews_CircleCheckid",
                table: "ComplaintViews",
                column: "CircleCheckid");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintViews_CircleChecks_CircleCheckid",
                table: "ComplaintViews",
                column: "CircleCheckid",
                principalTable: "CircleChecks",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExteriorViews_CircleChecks_CircleCheckid",
                table: "ExteriorViews",
                column: "CircleCheckid",
                principalTable: "CircleChecks",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_InteriorViews_CircleChecks_CircleCheckid",
                table: "InteriorViews",
                column: "CircleCheckid",
                principalTable: "CircleChecks",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_TireViews_CircleChecks_CircleCheckid",
                table: "TireViews",
                column: "CircleCheckid",
                principalTable: "CircleChecks",
                principalColumn: "id");
        }
    }
}
