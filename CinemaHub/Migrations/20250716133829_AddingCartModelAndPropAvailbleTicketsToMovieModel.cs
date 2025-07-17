using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaHub.Migrations
{
    /// <inheritdoc />
    public partial class AddingCartModelAndPropAvailbleTicketsToMovieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableTickets",
                table: "Movies",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTickets",
                table: "Movies");
        }
    }
}
