using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSuspendedAttouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSuspendedAt",
                table: "Utilisateurs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSuspendedAt",
                table: "Utilisateurs");
        }
    }
}
