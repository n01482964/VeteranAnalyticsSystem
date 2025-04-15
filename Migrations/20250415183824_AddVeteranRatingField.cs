using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeteranAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddVeteranRatingField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "Veterans",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "Veterans");
        }
    }
}
