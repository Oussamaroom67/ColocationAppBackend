using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class ProfilProprietaireService
    {
        private readonly ApplicationDbContext _context;

        public ProfilProprietaireService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
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
                AvatarUrl = proprietaire.AvatarUrl,
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

            // Validation des données
            if (string.IsNullOrWhiteSpace(request.Nom) || string.IsNullOrWhiteSpace(request.Prenom))
            {
                throw new Exception("Le nom et le prénom sont obligatoires");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new Exception("L'email est obligatoire");
            }

            // Vérifier que l'email n'est pas déjà utilisé par un autre utilisateur
            var existingUser = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != proprietaireId);

            if (existingUser != null)
            {
                throw new Exception("Cette adresse email est déjà utilisée");
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
            proprietaire.AvatarUrl = request.AvatarUrl;

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
                NombreProprietes = proprietaire.NombreProprietes,
                EstVerifie = proprietaire.EstVerifie,
                DateInscription = proprietaire.DateInscription,
                Status = proprietaire.Status.ToString()
            };
        }


 
    }
}