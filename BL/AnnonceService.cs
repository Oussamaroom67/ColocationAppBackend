using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AnnonceService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    private string baseUrl;

    public AnnonceService(ApplicationDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        this.baseUrl = _configuration["BaseUrl"];
    }

    public object? GetById(int id)
    {
        var annonce = _context.Annonces
            .Include(a => a.Logement)
                .ThenInclude(l => l.Proprietaire)
            .Include(a => a.Photos)
            .FirstOrDefault(a => a.Id == id);

        if (annonce == null) return null;

        annonce.NbVues++;
        _context.SaveChanges();
        var avis = _context.Avis
        .Where(a => a.ProprietaireId == annonce.Logement.ProprietaireId);

        double? noteGlobale = avis.Any() ? avis.Average(a => a.rating) : null;
        return new
        {
            annonce.Id,
            annonce.Titre,
            annonce.Description,
            annonce.Prix,
            annonce.Caution,
            annonce.Charges,
            annonce.DisponibleDe,
            annonce.DisponibleJusqu,
            annonce.DureeMinMois,
            annonce.OccupantsMax,
            annonce.Statut,
            Logement = new
            {
                annonce.Logement.Adresse,
                annonce.Logement.Ville,
                annonce.Logement.EstMeuble,
                annonce.Logement.Type,
                annonce.Logement.NbSallesBain,
                annonce.Logement.AnimauxAutorises,
                annonce.Logement.FumeurAutorise,
                annonce.Logement.ParkingDisponible,
                annonce.Logement.Surface,
                annonce.Logement.NbChambres,
                annonce.Logement.Pays,
                annonce.Logement.Latitude,
                annonce.Logement.Longitude,
                annonce.Logement.InternetInclus,
                annonce.Logement.ChargesIncluses
            },
            Proprietaire = new
            {
                annonce.Logement.ProprietaireId,
                annonce.Logement.Proprietaire.Nom,
                annonce.Logement.Proprietaire.Prenom,
                annonce.Logement.Proprietaire.Email,
                NoteGlobale= noteGlobale,
                annonce.Logement.Proprietaire.Telephone,
                AvatarUrl = $"{this.baseUrl}{annonce.Logement.Proprietaire.AvatarUrl}"
            },
            Photos = annonce.Photos.Select(p => new { p.Id, Url = $"{baseUrl}{p.Url}" })
        };
    }
    public  List<AnnonceResponse> GetSimilarAnnonces(int id)
    {
        var annonce =  _context.Annonces
            .Include(a=>a.Logement)
            .FirstOrDefault(a => a.Id == id);

        if (annonce == null )
            return null;

        var similarAnnonces = _context.Annonces
            .Where(a => a.Id != id &&
                        a.Logement.Type == annonce.Logement.Type &&
                        a.Logement.Ville == a.Logement.Ville &&
                        Math.Abs(a.Prix - annonce.Prix) < 1000
            )
            .Include(a=>a.Logement)
            .Include(a => a.Photos)
            .Select(a => new AnnonceResponse
                {
                   AnnonceId = a.Id,
                   LogementId = a.LogementId,
                   Title = a.Titre,
                   Type = a.Logement.Type,
                   Prix = a.Prix,
                   Ville = a.Logement.Ville,
                   Beds = a.Logement.NbChambres,
                   Baths = a.Logement.NbSallesBain,
                   Photos = a.Photos.Select(p => new PhotoDto { Url = $"{baseUrl}{p.Url}" ?? "https://wiratthungsong.com/wts/assets/img/default.png" }).ToList()
                })
            .Take(4)
            .ToList();

        return similarAnnonces;
    }

    private IActionResult NotFoundResult()
    {
        throw new NotImplementedException();
    }
}
