using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstActif",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Utilisateurs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<bool>(
                name: "EstActif",
                table: "Utilisateurs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
