using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaCasaBack.Migrations
{
    /// <inheritdoc />
    public partial class AddWaitlistToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Waitlist",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Waitlist",
                table: "Bookings");
        }
    }
}
