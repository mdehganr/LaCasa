using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaCasaBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelToIncludeBookingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Waitlist",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "Waitlist",
                table: "Bookings",
                type: "boolean",
                nullable: true);
        }
    }
}
