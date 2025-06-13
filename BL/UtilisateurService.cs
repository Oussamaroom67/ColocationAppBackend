using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class UtilisateurService
    {
        private readonly ApplicationDbContext _context;
        public UtilisateurService(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get all Users
        public async Task<List<UtilisateurDto>> GetAllUtilisateursAsync()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            var result = utilisateurs.Select(u => new UtilisateurDto
            {
                Id = u.Id,
                Nom = u.Nom + " " + u.Prenom,
                Email = u.Email,
                TypeUtilisateur = u is Etudiant ? "Étudiant" :
                  u is Proprietaire ? "Propriétaire" :
                  u is Administrateur ? "Administrateur" : "Inconnu",
                Statut = u.EstActif ? "Actif" : "Inactif",
                Verifie = u.EstVerifie ? "Vérifié" : "Non vérifié",
                DateInscription = u.DateInscription,
                NbrUtilisateur = utilisateurs.Count(),
                NbrProprietaire = utilisateurs.Count(r => r is Proprietaire),
                NbrEtudiants = utilisateurs.Count(r => r is Etudiant),
                NbrAdministrateurs = utilisateurs.Count(r => r is Administrateur)
            }).ToList();

            return result;
        }
         
    }
}
