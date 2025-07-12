
using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class OwnerProfile
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private string baseUrl;
        public OwnerProfile(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration=configuration;
            this.baseUrl = _configuration["BaseUrl"];

        }
        //getInfo
        public async Task<OwnerProfileDTO> getInfo(int id)
        {
            var prop =await _context.Proprietaires.FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null)
            {
                throw new Exception("Propriétaire introuvable");
            }
            double noteGlobale = 0;
            var avisList = await _context.Avis
                .Where(p => p.ProprietaireId == id)
                .ToListAsync();

            if (avisList.Any())
            {
                noteGlobale = avisList.Average(p => p.rating);
            }
            return new OwnerProfileDTO
            {
                Nom = prop.Nom +" "+ prop.Prenom,
                NmbProprietes = await _context.Logements.CountAsync(p => p.ProprietaireId == id),
                Adresse = prop.Adresse,
                AvatarProp = $"{baseUrl}{prop.AvatarUrl}",
                Note = noteGlobale,
                pays = prop.Pays,
                Ville = prop.Ville,
                Email = prop.Email,
                Phone = prop.Telephone,
                Avis = await _context.Avis
                .Where(a => a.ProprietaireId == id)
                .Select(a => new AvisResponseDTO
                {
                    NomEtudiant = a.Student.Nom + " " + a.Student.Prenom,
                    Rating = a.rating,
                    Comment = a.comment,
                    AvatarProfile = a.Student.AvatarUrl
                }).ToListAsync()
            };
        }
        public async Task AddAvis(AvisDto avis)
        {
            var student = await _context.Etudiants.FirstOrDefaultAsync(a => a.Id == avis.StudentId);
            if (student == null)
            {
                throw new ArgumentException("Etudiant non trouvable");
            }
            var prop = await _context.Proprietaires.FirstOrDefaultAsync(p => p.Id == avis.ProprietaireId);
            if (prop == null)
                throw new ArgumentException("Propriétaire introuvable.");
            bool dejaCommente = await _context.Avis.AnyAsync(a =>
                a.ProprietaireId == avis.ProprietaireId && a.StudentId == avis.StudentId);
            if (dejaCommente)
                throw new InvalidOperationException("Vous avez déjà laissé un avis pour ce propriétaire.");

            var avi = new AvisStudent
            {
                rating = avis.rating,
                comment = avis.comment,
                StudentId = avis.StudentId,
                ProprietaireId = avis.ProprietaireId
            };
            _context.Avis.Add(avi);
            await _context.SaveChangesAsync();
        }

    }
}
