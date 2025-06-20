using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class deleteIsFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Annonces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Annonces",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
