using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class ColocationManager
    {
        private readonly ApplicationDbContext _context;

        public ColocationManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ColocationResponse> AjouterColocationAsync(AjouterColocationRequest req, int etudiantId)
        {
            // Vérifie que l'étudiant existe
            var etudiant = await _context.Etudiants.FindAsync(etudiantId);
            if (etudiant == null)
                throw new Exception("Étudiant introuvable");

            var colocation = new Colocation
            {
                Adresse = req.Adresse,
                Budget = req.Budget,
                Type = req.Type,
                DateDebutDisponibilite = req.DateDebutDisponibilite,
                Preferences = req.Preferences ?? new List<string>(),
                EtudiantId = etudiantId
            };

            _context.Colocations.Add(colocation);
            await _context.SaveChangesAsync();

            return new ColocationResponse
            {
                ColocationId = colocation.Id,
                Adresse = colocation.Adresse,
                Budget = colocation.Budget,
                Type = colocation.Type.ToString(),
                DateDebutDisponibilite = colocation.DateDebutDisponibilite,
                Preferences = colocation.Preferences,
                NomEtudiant = $"{etudiant.Prenom} {etudiant.Nom}"
            };
        }
    }
}