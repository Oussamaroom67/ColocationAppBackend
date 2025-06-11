using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddColocationIdToDemandeColocation1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColocationId",
                table: "DemandesColocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Si nécessaire, ajouter une clé étrangère explicitement :
            migrationBuilder.AddForeignKey(
                name: "FK_DemandesColocation_Colocations_ColocationId",
                table: "DemandesColocation",
                column: "ColocationId",
                principalTable: "Colocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemandesColocation_Colocations_ColocationId",
                table: "DemandesColocation");

            migrationBuilder.DropColumn(
                name: "ColocationId",
                table: "DemandesColocation");
        }

    }
}
