using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaHub.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeatNumberFromCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "Carts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                columns: new[] { "ApplicationUserId", "MovieId", "MovieScheduleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                columns: new[] { "ApplicationUserId", "MovieId", "MovieScheduleId", "SeatNumber" });
        }
    }
}
