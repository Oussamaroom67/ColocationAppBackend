using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class GestionUtilisateurs
    {
        private readonly ApplicationDbContext _context;
        public GestionUtilisateurs(ApplicationDbContext context)
        {
            _context = context;
        }
        //get all Users
        public async Task<List<GestionUtilisateurDTO>> GetAllUsers()
        {
            var etudiants = await _context.Utilisateurs
            .OfType<Etudiant>()
            .ToListAsync();

            var proprietaires = await _context.Utilisateurs
                .OfType<Proprietaire>()
                .ToListAsync();

            var utilisateurs = etudiants.Cast<Utilisateur>()
                .Concat(proprietaires)
                .ToList();
            return  utilisateurs
                .Select(u => new GestionUtilisateurDTO
                {
                    id = u.Id,
                    Nom = u.Nom + " " + u.Prenom,
                    Email = u.Email,
                    Type = u is Etudiant ? "Etudiant" : u is Proprietaire ? "Proprietaire" : "Administrateur",
                    Statut = u.Status,
                    EstVerifie = u.EstVerifie,
                    DateInscription = u.DateInscription.ToString("yyyy-MM-dd"),
                    avatarUrl = u.AvatarUrl != null ? u.AvatarUrl : ""
                })
                .ToList();
        }

    }
}
