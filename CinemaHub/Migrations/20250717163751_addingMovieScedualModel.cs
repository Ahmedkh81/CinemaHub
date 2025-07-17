using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaHub.Migrations
{
    /// <inheritdoc />
    public partial class addingMovieScedualModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Movies_MovieId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.AddColumn<int>(
                name: "MovieScheduleId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MovieScheduleId1",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                columns: new[] { "ApplicationUserId", "MovieId", "MovieScheduleId", "SeatNumber" });

            migrationBuilder.CreateTable(
                name: "MovieSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ShowTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSchedules_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MovieScheduleId",
                table: "Carts",
                column: "MovieScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MovieScheduleId1",
                table: "Carts",
                column: "MovieScheduleId1");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSchedules_MovieId",
                table: "MovieSchedules",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_MovieSchedules_MovieScheduleId",
                table: "Carts",
                column: "MovieScheduleId",
                principalTable: "MovieSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_MovieSchedules_MovieScheduleId1",
                table: "Carts",
                column: "MovieScheduleId1",
                principalTable: "MovieSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Movies_MovieId",
                table: "Carts",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_MovieSchedules_MovieScheduleId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_MovieSchedules_MovieScheduleId1",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Movies_MovieId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "MovieSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MovieScheduleId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MovieScheduleId1",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "MovieScheduleId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "MovieScheduleId1",
                table: "Carts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                columns: new[] { "ApplicationUserId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Movies_MovieId",
                table: "Carts",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
