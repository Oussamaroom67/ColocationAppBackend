namespace ColocationAppBackend.Models
{
    public class OffreColocation
    {
        public int Id { get; set; }

        public string Titre { get; set; }
        public string Description { get; set; }

        public string Adresse { get; set; }

        public double Budget { get; set; }

        public int NombreChambresDisponibles { get; set; }
        public int NombreTotalChambres { get; set; }

        public DateTime DateDebutDisponibilite { get; set; }

        public List<string> Photos { get; set; } // Chemins des images stockés

        public string Conditions { get; set; } // Règles de vie, etc.
        public string TypeLogement { get; set; } // Appartement, maison, etc.

        public string SexeRecherche { get; set; } // Homme, Femme, Indifférent
        public int TrancheAgeMin { get; set; }
        public int TrancheAgeMax { get; set; }

        // Relations
        public int ProprietaireId { get; set; }
        public Proprietaire Proprietaire { get; set; }

        public List<Utilisateur> Colocataires { get; set; } // Étudiants déjà présents

        public OffreColocation()
        {
            Photos = new List<string>();
            Colocataires = new List<Utilisateur>();
        }

    }
}
