using Microsoft.EntityFrameworkCore;
using ColocationAppBackend.Models;

namespace ColocationAppBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet pour chaque entité
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour héritage TPH
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Etudiant>("Etudiant")
                .HasValue<Proprietaire>("Proprietaire")
                .HasValue<Administrateur>("Administrateur");

            // Tu pourras ajouter les relations (OneToMany, ManyToMany...) ici ensuite
            //part Omaima
            //part Oussama
        }
    }
}

