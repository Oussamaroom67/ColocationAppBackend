using Microsoft.EntityFrameworkCore;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace ColocationAppBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Déclarations des DbSet représentant les tables en base pour chaque entité
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Proprietaire> Proprietaires { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; }
        public DbSet<Logement> Logements { get; set; }
        public DbSet<Annonce> Annonces { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Colocation> Colocations { get; set; }
        public DbSet<DemandeColocation> DemandesColocation { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Signalement> Signalments { get; set; }
        public DbSet<ReseauSocial> ReseauxSociaux { get; set; }
        public DbSet<Favori> Favoris { get; set; }

        // Configuration des modèles et des relations entre entités
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Héritage TPH (Table Per Hierarchy) pour la classe Utilisateur et ses sous-classes
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Etudiant>("Etudiant")
                .HasValue<Proprietaire>("Proprietaire")
                .HasValue<Administrateur>("Administrateur");

            // Relation entre Conversation et Utilisateur1 (participant 1) avec suppression restreinte
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Utilisateur1)
                .WithMany(u => u.ConversationsParticipant1)
                .HasForeignKey(c => c.Utilisateur1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation entre Conversation et Utilisateur2 (participant 2) avec suppression restreinte
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Utilisateur2)
                .WithMany(u => u.ConversationsParticipant2)
                .HasForeignKey(c => c.Utilisateur2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation un-à-plusieurs entre Proprietaire et Logements, suppression en cascade
            modelBuilder.Entity<Proprietaire>()
                .HasMany(p => p.Logements)
                .WithOne(l => l.Proprietaire)
                .HasForeignKey(l => l.ProprietaireId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs entre Annonce et Photos, suppression en cascade
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Annonce)
                .WithMany(a => a.Photos)
                .HasForeignKey(p => p.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs entre Etudiant et Colocations, suppression en cascade
            modelBuilder.Entity<Colocation>()
                .HasOne(c => c.Etudiant)
                .WithMany(e => e.Colocations)
                .HasForeignKey(c => c.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs entre Etudiant et DemandesColocations, suppression en cascade
            modelBuilder.Entity<DemandeColocation>()
                .HasOne(d => d.Etudiant)
                .WithMany(e => e.DemandesColocations)
                .HasForeignKey(d => d.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs entre Etudiant et Favoris, suppression en cascade
            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Etudiant)
                .WithMany(e => e.Favoris)
                .HasForeignKey(f => f.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs entre Conversation et Messages, suppression en cascade
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Expediteur)
                .WithMany() // Si tu ne veux pas de collection inverse dans Utilisateur
                .HasForeignKey(m => m.ExpediteurId)
                .OnDelete(DeleteBehavior.Restrict); 


            // Relations pour Signalement

            // Relation Signalement -> Signaleur (utilisateur qui signale), suppression restreinte
            modelBuilder.Entity<Signalement>()
                .HasOne(s => s.Signaleur)
                .WithMany(u => u.SignalementsEnvoyes)
                .HasForeignKey(s => s.SignaleurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Signalement -> UtilisateurSignale (utilisateur signalé), suppression restreinte
            modelBuilder.Entity<Signalement>()
                .HasOne(s => s.UtilisateurSignale)
                .WithMany(u => u.SignalementsRecus)
                .HasForeignKey(s => s.UtilisateurSignaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Signalement -> ResoluPar (utilisateur ayant traité le signalement), suppression fixée à null
            modelBuilder.Entity<Signalement>()
                .HasOne(s => s.ResoluPar)
                .WithMany(u => u.SignalementsTraites)
                .HasForeignKey(s => s.ResoluParId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relation Signalement -> AnnonceSignalee (annonce signalée), suppression fixée à null
            modelBuilder.Entity<Signalement>()
                .HasOne(s => s.AnnonceSignalee)
                .WithMany(a => a.Signalements)
                .HasForeignKey(s => s.AnnonceSignaleeId)
                .OnDelete(DeleteBehavior.Cascade);
            // Relation un-à-plusieurs entre Etudiant et ReseauSocial, suppression en cascade
            modelBuilder.Entity<ReseauSocial>()
                .HasOne(r => r.Etudiant)
                .WithMany(e => e.ReseauxSociaux)
                .HasForeignKey(r => r.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Fabrique pour la création du DbContext en mode design-time (migrations)
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);

                return new ApplicationDbContext(optionsBuilder.Options);
            }

        }
    }
}
