using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class AnnonceModificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string baseUrl;

        public AnnonceModificationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            this.baseUrl = _configuration["BaseUrl"];
        }

        public async Task<List<AnnonceCompleteDto>> GetAnnoncesByProprietaireAsync(int proprietaireId)
        {
            var annonces = await _context.Annonces
                .Include(a => a.Logement)
                    .ThenInclude(l => l.Proprietaire)
                .Include(a => a.Photos)
                .Include(a => a.Signalements)
                .Include(a => a.DemandesLocation)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .ToListAsync();

            return annonces.Select(a => new AnnonceCompleteDto
            {
                // Données de l'annonce
                AnnonceId = a.Id,
                Titre = a.Titre,
                Description = a.Description,
                Prix = a.Prix,
                Caution = a.Caution,
                Charges = a.Charges,
                DisponibleDe = a.DisponibleDe,
                DisponibleJusqu = a.DisponibleJusqu,
                DureeMinMois = a.DureeMinMois,
                OccupantsMax = a.OccupantsMax,
                StatutAnnonce = a.Statut,
                NbVues = a.NbVues,
                DateModification = a.DateModification,
                StatutVerification = a.StatutVerification,

                // Données du logement
                LogementId = a.Logement.Id,
                Adresse = a.Logement.Adresse,
                Surface = a.Logement.Surface,
                NbChambres = a.Logement.NbChambres,
                Ville = a.Logement.Ville,
                CodePostal = a.Logement.CodePostal,
                Pays = a.Logement.Pays,
                Latitude = a.Logement.Latitude,
                Longitude = a.Logement.Longitude,
                NbSallesBain = a.Logement.NbSallesBain,
                Etage = a.Logement.Etage,
                NbEtagesTotal = a.Logement.NbEtagesTotal,
                EstMeuble = a.Logement.EstMeuble,
                AnimauxAutorises = a.Logement.AnimauxAutorises,
                FumeurAutorise = a.Logement.FumeurAutorise,
                InternetInclus = a.Logement.InternetInclus,
                ChargesIncluses = a.Logement.ChargesIncluses,
                ParkingDisponible = a.Logement.ParkingDisponible,
                StatutLogement = a.Logement.status,
                TypeLogement = a.Logement.Type,

                // Données du propriétaire
                ProprietaireId = a.Logement.Proprietaire.Id,
                ProprietaireNom = a.Logement.Proprietaire.Nom,
                ProprietairePrenom = a.Logement.Proprietaire.Prenom,
                ProprietaireEmail = a.Logement.Proprietaire.Email,
                ProprietaireTelephone = a.Logement.Proprietaire.Telephone,

                // Listes des relations
                Photos = a.Photos?.Select(p => new PhotoInfo
                {
                    Id = p.Id,
                    Url = $"{baseUrl}{p.Url}",
                }).ToList() ?? new List<PhotoInfo>(),



            }).ToList();
        }

        public async Task<AnnonceCompleteDto?> GetAnnonceCompleteAsync(int annonceId)
        {
            var annonce = await _context.Annonces
                .Include(a => a.Logement)
                    .ThenInclude(l => l.Proprietaire)
                .Include(a => a.Photos)
                .Include(a => a.Signalements)
                .Include(a => a.DemandesLocation)
                .FirstOrDefaultAsync(a => a.Id == annonceId);

            if (annonce == null) return null;

            return new AnnonceCompleteDto
            {
                // Données de l'annonce
                AnnonceId = annonce.Id,
                Titre = annonce.Titre,
                Description = annonce.Description,
                Prix = annonce.Prix,
                Caution = annonce.Caution,
                Charges = annonce.Charges,
                DisponibleDe = annonce.DisponibleDe,
                DisponibleJusqu = annonce.DisponibleJusqu,
                DureeMinMois = annonce.DureeMinMois,
                OccupantsMax = annonce.OccupantsMax,
                StatutAnnonce = annonce.Statut,
                NbVues = annonce.NbVues,
                DateModification = annonce.DateModification,
                StatutVerification = annonce.StatutVerification,

                // Données du logement
                LogementId = annonce.Logement.Id,
                Adresse = annonce.Logement.Adresse,
                Surface = annonce.Logement.Surface,
                NbChambres = annonce.Logement.NbChambres,
                Ville = annonce.Logement.Ville,
                CodePostal = annonce.Logement.CodePostal,
                Pays = annonce.Logement.Pays,
                Latitude = annonce.Logement.Latitude,
                Longitude = annonce.Logement.Longitude,
                NbSallesBain = annonce.Logement.NbSallesBain,
                Etage = annonce.Logement.Etage,
                NbEtagesTotal = annonce.Logement.NbEtagesTotal,
                EstMeuble = annonce.Logement.EstMeuble,
                AnimauxAutorises = annonce.Logement.AnimauxAutorises,
                FumeurAutorise = annonce.Logement.FumeurAutorise,
                InternetInclus = annonce.Logement.InternetInclus,
                ChargesIncluses = annonce.Logement.ChargesIncluses,
                ParkingDisponible = annonce.Logement.ParkingDisponible,
                StatutLogement = annonce.Logement.status,
                TypeLogement = annonce.Logement.Type,

                // Données du propriétaire
                ProprietaireId = annonce.Logement.Proprietaire.Id,
                ProprietaireNom = annonce.Logement.Proprietaire.Nom,
                ProprietairePrenom = annonce.Logement.Proprietaire.Prenom,
                ProprietaireEmail = annonce.Logement.Proprietaire.Email,
                ProprietaireTelephone = annonce.Logement.Proprietaire.Telephone,

                // Listes des relations
                Photos = annonce.Photos?.Select(p => new PhotoInfo
                {
                    Id = p.Id,
                    Url = $"{baseUrl}{p.Url}",
                }).ToList() ?? new List<PhotoInfo>(),


            };
        }
    }
}