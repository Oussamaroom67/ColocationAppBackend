using ColocationAppBackend.Data;
using Microsoft.EntityFrameworkCore;

public class AnnonceService
{
    private readonly ApplicationDbContext _context;

    public AnnonceService(ApplicationDbContext context)
    {
        _context = context;
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
                annonce.Logement.Proprietaire.Nom,
                annonce.Logement.Proprietaire.Prenom,
                annonce.Logement.Proprietaire.Email,
                annonce.Logement.Proprietaire.NoteGlobale
            },
            Photos = annonce.Photos.Select(p => new { p.Id, p.Url })
        };
    }
}
