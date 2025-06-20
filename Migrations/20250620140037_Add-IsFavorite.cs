using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColocationAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateInscription = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EstVerifie = table.Column<bool>(type: "bit", nullable: false),
                    DernierConnexion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSuspendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    NiveauEtudes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Etudiant_Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Universite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomaineEtudes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Habitudes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CentresInteret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StyleDeVie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteGlobale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NombreProprietes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Budget = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DateDebutDisponibilite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Preferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EtudiantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Colocations_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Utilisateur1Id = table.Column<int>(type: "int", nullable: false),
                    Utilisateur2Id = table.Column<int>(type: "int", nullable: false),
                    DernierMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateDernierMessage = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_Utilisateurs_Utilisateur1Id",
                        column: x => x.Utilisateur1Id,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Utilisateurs_Utilisateur2Id",
                        column: x => x.Utilisateur2Id,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surface = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NbChambres = table.Column<int>(type: "int", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    NbSallesBain = table.Column<int>(type: "int", nullable: false),
                    Etage = table.Column<int>(type: "int", nullable: true),
                    NbEtagesTotal = table.Column<int>(type: "int", nullable: true),
                    EstMeuble = table.Column<bool>(type: "bit", nullable: false),
                    AnimauxAutorises = table.Column<bool>(type: "bit", nullable: false),
                    FumeurAutorise = table.Column<bool>(type: "bit", nullable: false),
                    InternetInclus = table.Column<bool>(type: "bit", nullable: false),
                    ChargesIncluses = table.Column<bool>(type: "bit", nullable: false),
                    ParkingDisponible = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProprietaireId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logements_Utilisateurs_ProprietaireId",
                        column: x => x.ProprietaireId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReseauxSociaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EtudiantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReseauxSociaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReseauxSociaux_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemandesColocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateEmmenagement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Preferences = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ReponseProprietaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReponse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EtudiantId = table.Column<int>(type: "int", nullable: false),
                    ColocationId = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesColocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesColocation_Colocations_ColocationId",
                        column: x => x.ColocationId,
                        principalTable: "Colocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DemandesColocation_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpediteurId = table.Column<int>(type: "int", nullable: false),
                    Contenu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateEnvoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLecture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstLu = table.Column<bool>(type: "bit", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Utilisateurs_ExpediteurId",
                        column: x => x.ExpediteurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Annonces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Caution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DisponibleDe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisponibleJusqu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DureeMinMois = table.Column<int>(type: "int", nullable: false),
                    OccupantsMax = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    NbVues = table.Column<int>(type: "int", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    LogementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annonces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annonces_Logements_LogementId",
                        column: x => x.LogementId,
                        principalTable: "Logements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemandesLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnonceId = table.Column<int>(type: "int", nullable: false),
                    EtudiantId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateEmmenagement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DureeSejour = table.Column<int>(type: "int", nullable: false),
                    NbOccupants = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    MessageReponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReponse = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesLocation_Annonces_AnnonceId",
                        column: x => x.AnnonceId,
                        principalTable: "Annonces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DemandesLocation_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtudiantId = table.Column<int>(type: "int", nullable: false),
                    AnnonceId = table.Column<int>(type: "int", nullable: true),
                    OffreColocationId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DateAjout = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoris_Annonces_AnnonceId",
                        column: x => x.AnnonceId,
                        principalTable: "Annonces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favoris_Colocations_OffreColocationId",
                        column: x => x.OffreColocationId,
                        principalTable: "Colocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favoris_Utilisateurs_EtudiantId",
                        column: x => x.EtudiantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAjout = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnnonceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Annonces_AnnonceId",
                        column: x => x.AnnonceId,
                        principalTable: "Annonces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Signalments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Motif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSignalement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignaleurId = table.Column<int>(type: "int", nullable: false),
                    UtilisateurSignaleId = table.Column<int>(type: "int", nullable: false),
                    AnnonceSignaleeId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    NotesAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResoluParId = table.Column<int>(type: "int", nullable: true),
                    DateResolution = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signalments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signalments_Annonces_AnnonceSignaleeId",
                        column: x => x.AnnonceSignaleeId,
                        principalTable: "Annonces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Signalments_Utilisateurs_ResoluParId",
                        column: x => x.ResoluParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Signalments_Utilisateurs_SignaleurId",
                        column: x => x.SignaleurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Signalments_Utilisateurs_UtilisateurSignaleId",
                        column: x => x.UtilisateurSignaleId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Annonces_LogementId",
                table: "Annonces",
                column: "LogementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colocations_EtudiantId",
                table: "Colocations",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Utilisateur1Id",
                table: "Conversations",
                column: "Utilisateur1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Utilisateur2Id",
                table: "Conversations",
                column: "Utilisateur2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesColocation_ColocationId",
                table: "DemandesColocation",
                column: "ColocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesColocation_EtudiantId",
                table: "DemandesColocation",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesLocation_AnnonceId",
                table: "DemandesLocation",
                column: "AnnonceId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesLocation_EtudiantId",
                table: "DemandesLocation",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_AnnonceId",
                table: "Favoris",
                column: "AnnonceId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_EtudiantId",
                table: "Favoris",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_OffreColocationId",
                table: "Favoris",
                column: "OffreColocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logements_ProprietaireId",
                table: "Logements",
                column: "ProprietaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ExpediteurId",
                table: "Messages",
                column: "ExpediteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AnnonceId",
                table: "Photos",
                column: "AnnonceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReseauxSociaux_EtudiantId",
                table: "ReseauxSociaux",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Signalments_AnnonceSignaleeId",
                table: "Signalments",
                column: "AnnonceSignaleeId");

            migrationBuilder.CreateIndex(
                name: "IX_Signalments_ResoluParId",
                table: "Signalments",
                column: "ResoluParId");

            migrationBuilder.CreateIndex(
                name: "IX_Signalments_SignaleurId",
                table: "Signalments",
                column: "SignaleurId");

            migrationBuilder.CreateIndex(
                name: "IX_Signalments_UtilisateurSignaleId",
                table: "Signalments",
                column: "UtilisateurSignaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemandesColocation");

            migrationBuilder.DropTable(
                name: "DemandesLocation");

            migrationBuilder.DropTable(
                name: "Favoris");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "ReseauxSociaux");

            migrationBuilder.DropTable(
                name: "Signalments");

            migrationBuilder.DropTable(
                name: "Colocations");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Annonces");

            migrationBuilder.DropTable(
                name: "Logements");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
