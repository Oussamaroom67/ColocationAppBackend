using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class migration_signalements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignaleurName",
                table: "Signalments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UtilisateurSignaleName",
                table: "Signalments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignaleurName",
                table: "Signalments");

            migrationBuilder.DropColumn(
                name: "UtilisateurSignaleName",
                table: "Signalments");
        }
    }
}
