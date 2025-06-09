using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class ProprietaireManager
    {
        private readonly ApplicationDbContext _context;

        public ProprietaireManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AnnonceResponse> AjouterLogementEtAnnonceAsync(LogementAnnonceRequest req)
        {
            // Vérifie que le propriétaire existe
            var proprietaire = await _context.Proprietaires.FindAsync(req.ProprietaireId);
            if (proprietaire == null)
                throw new Exception("Propriétaire introuvable");

            // Vérifie si le logement existe déjà pour ce propriétaire à la même adresse
            var logementExistant = await _context.Logements
                .Include(l => l.Annonce)
                .FirstOrDefaultAsync(l =>
                    l.Adresse.ToLower() == req.Address.ToLower() &&
                    l.CodePostal == req.PostalCode &&
                    l.ProprietaireId == req.ProprietaireId
                );

            if (logementExistant != null && logementExistant.Annonce != null)
                throw new Exception("Une annonce existe déjà pour ce logement.");

            // Crée l’annonce
            var annonce = new Annonce
            {
                Titre = req.Title,
                Description = req.Description,
                Prix = req.MonthlyRent,
                Caution = req.Deposit,
                Charges = req.Fees,
                DisponibleDe = DateTime.Parse(req.AvailableFrom),
                DisponibleJusqu = DateTime.Now.AddMonths(12),
                DureeMinMois = int.TryParse(req.DesiredDuration, out int d) ? d : 1,
                OccupantsMax = 1,
                Statut = AnnonceStatus.Brouillon,
                NbVues = 0,
                DateModification = DateTime.Now,
                Photos = req.Photos?.Select(p => new Photo
                {
                    Url = p.Url,
                    DateAjout = DateTime.Now
                }).ToList()
            };

            Logement logement;

            if (logementExistant != null)
            {
                logement = logementExistant;
                logement.Annonce = annonce;
            }
            else
            {
                logement = new Logement
                {
                    Adresse = req.Address,
                    Ville = req.City,
                    CodePostal = req.PostalCode,
                    Pays = req.Country,
                    Surface = req.Surface,
                    NbChambres = req.Bedrooms,
                    NbSallesBain = req.Bathrooms,
                    Etage = req.Floor,
                    EstMeuble = req.Furnished,
                    AnimauxAutorises = req.PetsAllowed,
                    FumeurAutorise = req.SmokingAllowed,
                    InternetInclus = req.Amenities?.Contains("internet") ?? false,
                    ChargesIncluses = req.Amenities?.Contains("charges") ?? false,
                    ParkingDisponible = req.Amenities?.Contains("parking") ?? false,
                    ProprietaireId = req.ProprietaireId,
                    Annonce = annonce
                };

                _context.Logements.Add(logement);
            }

            await _context.SaveChangesAsync();

            return new AnnonceResponse
            {
                AnnonceId = annonce.Id,
                LogementId = logement.Id,
                Title = annonce.Titre,
                Prix = annonce.Prix,
                Ville = logement.Ville,
                Photos = annonce.Photos?.Select(p => new PhotoDto
                {
                    Url = p.Url,
                    Name = System.IO.Path.GetFileName(p.Url)
                }).ToList()
            };
        }
    }
}
