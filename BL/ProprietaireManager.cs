using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ColocationAppBackend.BL
{
    public class ProprietaireManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public ProprietaireManager(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<string> SaveImageAsync(string base64Image, string fileName)
        {
            try
            {
                // Nettoie le base64 (retire data:image/jpeg;base64,)
                var base64Data = base64Image.Contains(',')
                    ? base64Image.Split(',')[1]
                    : base64Image;

                // Convertit en bytes
                var imageBytes = Convert.FromBase64String(base64Data);

                // Génère un nom unique
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                // Chemin de sauvegarde
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties");

                // Crée le dossier s'il n'existe pas
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Chemin complet du fichier
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Sauvegarde le fichier
                await File.WriteAllBytesAsync(filePath, imageBytes);

                // Retourne l'URL relative
                return $"/images/properties/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde de l'image: {ex.Message}");
            }
        }

        public async Task<AnnonceResponse> AjouterLogementEtAnnonceAsync(LogementAnnonceRequest req)
        {
            // Vérifie que le propriétaire existe
            var proprietaire = await _context.Proprietaires.FindAsync(req.ProprietaireId);
            if (proprietaire == null)
                throw new Exception("Propriétaire introuvable");

            Logement logement;
            Annonce annonce;

            // Traitement des photos - sauvegarde des images base64
            List<Photo> photosToSave = new List<Photo>();
            if (req.Photos != null && req.Photos.Any())
            {
                foreach (var photo in req.Photos)
                {
                    string imageUrl;

                    // Si c'est une image base64, on la sauvegarde
                    if (photo.Url.StartsWith("data:image/"))
                    {
                        var fileName = !string.IsNullOrEmpty(photo.Name) ? photo.Name : "image.jpg";
                        imageUrl = await SaveImageAsync(photo.Url, fileName);
                    }
                    else
                    {
                        // Si c'est déjà une URL, on la garde telle quelle
                        imageUrl = photo.Url;
                    }

                    photosToSave.Add(new Photo
                    {
                        Url = imageUrl,
                        DateAjout = DateTime.Now
                    });
                }
            }

            if (req.LogementId.HasValue)
            {
                // MODE MODIFICATION
                logement = await _context.Logements
                    .Include(l => l.Annonce)
                    .ThenInclude(a => a.Photos)
                    .FirstOrDefaultAsync(l => l.Id == req.LogementId.Value && l.ProprietaireId == req.ProprietaireId);

                if (logement == null)
                    throw new Exception("Logement introuvable ou vous n'avez pas les droits pour le modifier");

                // Mettre à jour les propriétés du logement
                logement.Adresse = req.Address;
                logement.Ville = req.City;
                logement.CodePostal = req.PostalCode;
                logement.Pays = req.Country;
                logement.Surface = req.Surface;
                logement.NbChambres = req.Bedrooms;
                logement.NbSallesBain = req.Bathrooms;
                logement.Etage = req.Floor;
                logement.EstMeuble = req.Furnished;
                logement.AnimauxAutorises = req.PetsAllowed;
                logement.FumeurAutorise = req.SmokingAllowed;
                logement.InternetInclus = req.Amenities?.Contains("internet") ?? false;
                logement.ChargesIncluses = req.Amenities?.Contains("charges") ?? false;
                logement.ParkingDisponible = req.Amenities?.Contains("parking") ?? false;
                logement.Type = req.Type;

                // Mettre à jour l'annonce si elle existe
                if (logement.Annonce != null)
                {
                    annonce = logement.Annonce;
                    annonce.Titre = req.Title;
                    annonce.Description = req.Description;
                    annonce.Prix = req.MonthlyRent;
                    annonce.Caution = req.Deposit;
                    annonce.Charges = req.Fees;
                    annonce.DisponibleDe = DateTime.Parse(req.AvailableFrom);
                    annonce.DureeMinMois = int.TryParse(req.DesiredDuration, out int d) ? d : 1;
                    annonce.DateModification = DateTime.Now;

                    // Supprimer les anciennes photos et ajouter les nouvelles
                    if (annonce.Photos != null)
                    {
                        _context.Photos.RemoveRange(annonce.Photos);
                    }
                    annonce.Photos = photosToSave;
                }
                else
                {
                    // Créer une nouvelle annonce pour ce logement
                    annonce = new Annonce
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
                        Photos = photosToSave
                    };
                    logement.Annonce = annonce;
                }
            }
            else
            {
                // MODE CRÉATION
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

                // Crée l'annonce
                annonce = new Annonce
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
                    Statut = req.Status != default ? req.Status : AnnonceStatus.Brouillon,
                    NbVues = 0,
                    DateModification = DateTime.Now,
                    Photos = photosToSave
                };

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
                        Annonce = annonce,
                        Type = req.Type,
                    };

                    _context.Logements.Add(logement);
                }
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
        public async Task<IEnumerable<AnnonceSummaryDto>> GetAnnoncesByProprietaireIdAsync(int proprietaireId)
        {
            string? baseUrl = _configuration["BaseUrl"];

            return await _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .Select(a => new AnnonceSummaryDto
                {
                    Id = a.Id,
                    Title = a.Titre,
                    Location = $"{a.Logement.Ville}",
                    PublishDate = "Publié le " + a.DateModification.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR")),
                    Price = $"{a.Prix} MAD/mois",
                    Views = a.NbVues,
                    Status = a.Statut.ToString().ToLower(),
                    ImageUrl = a.Photos.Any()
                    ? $"{baseUrl}{a.Photos.First().Url}"
                    : "https://wiratthungsong.com/wts/assets/img/default.png"
                })
                .ToListAsync();
        }
        public async Task<bool> SupprimerLogementAsync(int logementId, int proprietaireId)
        {
            var logement = await _context.Logements
                .Include(l => l.Annonce)
                .FirstOrDefaultAsync(l => l.Id == logementId && l.ProprietaireId == proprietaireId);

            if (logement == null)
                throw new Exception("Logement introuvable ou vous n'avez pas les droits pour le supprimer");

            if (logement.Annonce != null)
            {
                // Supprimer les DemandesLocation liées à l’annonce
                var demandes = _context.DemandesLocation.Where(d => d.AnnonceId == logement.Annonce.Id);
                _context.DemandesLocation.RemoveRange(demandes);

                // Supprimer les Favoris liés à l’annonce
                var favoris = _context.Favoris.Where(f => f.AnnonceId == logement.Annonce.Id);
                _context.Favoris.RemoveRange(favoris);

                // Supprimer les Photos liées à l’annonce
                var photos = _context.Photos.Where(p => p.AnnonceId == logement.Annonce.Id);
                _context.Photos.RemoveRange(photos);

                // Supprimer l’annonce elle-même
                _context.Annonces.Remove(logement.Annonce);
            }

            _context.Logements.Remove(logement);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> ChangerStatutAnnonceAsync(int annonceId, AnnonceStatus nouveauStatut, int proprietaireId)
        {
            // Vérifie que l'annonce existe et appartient au propriétaire
            var annonce = await _context.Annonces
                .Include(a => a.Logement)
                .FirstOrDefaultAsync(a => a.Id == annonceId && a.Logement.ProprietaireId == proprietaireId);

            if (annonce == null)
                throw new Exception("Annonce introuvable ou vous n'avez pas les droits pour la modifier");

            // Change le statut
            annonce.Statut = nouveauStatut;
            annonce.DateModification = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
