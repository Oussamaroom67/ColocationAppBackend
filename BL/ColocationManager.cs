using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
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
        public async Task<DemandeColocationResponse> PostulerColocationAsync(PostulerColocationRequest req, int etudiantId)
        {
            // Vérifie que l'étudiant existe
            var etudiant = await _context.Etudiants.FindAsync(etudiantId);
            if (etudiant == null)
                throw new Exception("Étudiant introuvable");

            // Vérifie que la colocation existe
            var colocation = await _context.Colocations
                .Include(c => c.Etudiant)
                .FirstOrDefaultAsync(c => c.Id == req.ColocationId);
            if (colocation == null)
                throw new Exception("Colocation introuvable");

            // Vérifie que l'étudiant ne postule pas à sa propre colocation
            if (colocation.EtudiantId == etudiantId)
                throw new Exception("Vous ne pouvez pas postuler à votre propre colocation");

            // Vérifie si l'étudiant n'a pas déjà postulé à cette colocation
            var demandeExistante = await _context.DemandesColocation
                .FirstOrDefaultAsync(d => d.ColocationId == req.ColocationId && d.EtudiantId == etudiantId);
            if (demandeExistante != null)
                throw new Exception("Vous avez déjà postulé à cette colocation");

            var demandeColocation = new DemandeColocation
            {
                Budget = req.Budget,
                Adresse = req.Adresse,
                Message = req.Message,
                DateEmmenagement = req.DateEmmenagement,
                Preferences = req.Preferences ?? new List<string>(),
                Statut = StatutDemande.EnAttente,
                EtudiantId = etudiantId,
                ColocationId = req.ColocationId
            };

            _context.DemandesColocation.Add(demandeColocation);
            await _context.SaveChangesAsync();

            return new DemandeColocationResponse
            {
                DemandeId = demandeColocation.Id,
                Budget = demandeColocation.Budget,
                Adresse = demandeColocation.Adresse,
                Message = demandeColocation.Message,
                DateEmmenagement = demandeColocation.DateEmmenagement,
                Preferences = demandeColocation.Preferences,
                Statut = demandeColocation.Statut.ToString(),
                NomEtudiant = $"{etudiant.Prenom} {etudiant.Nom}",
                ColocationAdresse = colocation.Adresse,
                ProprietaireColocation = $"{colocation.Etudiant.Prenom} {colocation.Etudiant.Nom}"
            };
        }
    }
}