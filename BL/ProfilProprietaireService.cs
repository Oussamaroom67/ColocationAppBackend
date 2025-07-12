using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Threading.Tasks;

namespace ColocationAppBackend.BL
{
    public class ProfilProprietaireService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private string baseUrl;
        public ProfilProprietaireService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            this.baseUrl = _configuration["BaseUrl"];
        }

        private async Task<string> SaveImageAsync(string base64Image, string fileName)
        {
            try
            {
                var base64Data = base64Image.Contains(',')
                    ? base64Image.Split(',')[1]
                    : base64Image;

                var imageBytes = Convert.FromBase64String(base64Data);

                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                return $"/images/profiles/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde de l'image: {ex.Message}");
            }
        }

        public async Task<ProfilProprietaireDto> GetProfilAsync(int proprietaireId)
        {
            var proprietaire = await _context.Proprietaires
                .Include(p => p.Logements)
                .FirstOrDefaultAsync(p => p.Id == proprietaireId);

            if (proprietaire == null)
            {
                throw new Exception("Propriétaire non trouvé");
            }

            return new ProfilProprietaireDto
            {
                Id = proprietaire.Id,
                Nom = proprietaire.Nom,
                Prenom = proprietaire.Prenom,
                Email = proprietaire.Email,
                Telephone = proprietaire.Telephone,
                Adresse = proprietaire.Adresse,
                Ville = proprietaire.Ville,
                CodePostal = proprietaire.CodePostal,
                Pays = proprietaire.Pays,
                AvatarUrl = $"{baseUrl}{proprietaire.AvatarUrl }",
                NoteGlobale = proprietaire.NoteGlobale,
                NombreProprietes = proprietaire.Logements.Count,
                EstVerifie = proprietaire.EstVerifie,
                DateInscription = proprietaire.DateInscription,
                Status = proprietaire.Status.ToString()
            };
        }

        public async Task<ProfilProprietaireDto> UpdateProfilAsync(int proprietaireId, UpdateProfilProprietaireRequest request)
        {
            var proprietaire = await _context.Proprietaires
                .FirstOrDefaultAsync(p => p.Id == proprietaireId);

            if (proprietaire == null)
            {
                throw new Exception("Propriétaire non trouvé");
            }

            if (string.IsNullOrWhiteSpace(request.Nom) || string.IsNullOrWhiteSpace(request.Prenom))
            {
                throw new Exception("Le nom et le prénom sont obligatoires");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new Exception("L'email est obligatoire");
            }

            var existingUser = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != proprietaireId);

            if (existingUser != null)
            {
                throw new Exception("Cette adresse email est déjà utilisée");
            }

            // Gestion de l'avatar : si base64, sauvegarder l'image
            string avatarUrl = request.AvatarUrl;
            if (!string.IsNullOrEmpty(request.AvatarUrl) && request.AvatarUrl.StartsWith("data:image/"))
            {
                // Extraire extension
                var fileExtension = "jpg";
                var fileName = $"{Guid.NewGuid()}.{fileExtension}";

                avatarUrl = await SaveImageAsync(request.AvatarUrl, fileName);
            }

            // Mise à jour des données
            proprietaire.Nom = request.Nom;
            proprietaire.Prenom = request.Prenom;
            proprietaire.Email = request.Email;
            proprietaire.Telephone = request.Telephone;
            proprietaire.Adresse = request.Adresse;
            proprietaire.Ville = request.Ville;
            proprietaire.CodePostal = request.CodePostal;
            proprietaire.Pays = request.Pays;
            proprietaire.DateModification = DateTime.Now;
            proprietaire.AvatarUrl = avatarUrl;

            await _context.SaveChangesAsync();

            return new ProfilProprietaireDto
            {
                Id = proprietaire.Id,
                Nom = proprietaire.Nom,
                Prenom = proprietaire.Prenom,
                Email = proprietaire.Email,
                Telephone = proprietaire.Telephone,
                Adresse = proprietaire.Adresse,
                Ville = proprietaire.Ville,
                CodePostal = proprietaire.CodePostal,
                Pays = proprietaire.Pays,
                AvatarUrl = proprietaire.AvatarUrl,
                NoteGlobale = proprietaire.NoteGlobale,
                NombreProprietes = proprietaire.Logements?.Count ?? 0,
                EstVerifie = proprietaire.EstVerifie,
                DateInscription = proprietaire.DateInscription,
                Status = proprietaire.Status.ToString()
            };
        }
    }
}
