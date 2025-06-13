using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using ColocationAppBackend.Utils;
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

        public async Task<List<MesDemandesColocationResponse>> ObtenirMesDemandesAsync(int etudiantId)
        {
            var demandes = await _context.DemandesColocation
                .Include(d => d.Colocation)
                .ThenInclude(c => c.Etudiant)
                .Where(d => d.EtudiantId == etudiantId)
                .OrderByDescending(d => d.Id)
                .ToListAsync();

            return demandes.Select(d => new MesDemandesColocationResponse
            {
                Id = d.Id,
                Nom = $"{d.Colocation.Etudiant.Prenom} {d.Colocation.Etudiant.Nom}",
                Ecole = d.Colocation.Etudiant.Universite,
                Message = d.Message,
                Date = d.DateEmmenagement.ToString("yyyy-MM-dd"),
                Statut = EnumHelper.GetStatutFrancais(d.Statut), 
                Budget = d.Budget?.ToString("0") + " MAD/mois",
                Quartier = d.Adresse,
                Preferences = d.Preferences,
                Reponse = d.ReponseProprietaire,
                ColocationId = d.ColocationId,
                ColocationAdresse = d.Colocation.Adresse,
                ColocationBudget = d.Colocation.Budget.ToString("0") + " MAD/mois",
                ColocationPreferences = d.Colocation.Preferences
            }).ToList();
        }

        //  méthode pour répondre à une demande (pour le propriétaire)
        public async Task<bool> RepondreDemandeAsync(RepondreDemandeRequest request, int proprietaireId)
        {
            var demande = await _context.DemandesColocation
                .Include(d => d.Colocation)
                .FirstOrDefaultAsync(d => d.Id == request.DemandeId);

            if (demande == null)
                throw new Exception("Demande introuvable");

            // Vérifier que c'est bien le propriétaire de la colocation qui répond
            if (demande.Colocation.EtudiantId != proprietaireId)
                throw new Exception("Vous n'êtes pas autorisé à répondre à cette demande");

            // Vérifier que la demande est encore en attente
            if (demande.Statut != StatutDemande.EnAttente)
                throw new Exception("Cette demande a déjà été traitée");

            // Mettre à jour la demande
            demande.Statut = request.NouveauStatut;
            demande.ReponseProprietaire = request.ReponseMessage;
            demande.DateReponse = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        // 5. Méthode pour annuler une demande (pour l'étudiant)
        public async Task<bool> AnnulerDemandeAsync(int demandeId, int etudiantId)
        {
            var demande = await _context.DemandesColocation
                .FirstOrDefaultAsync(d => d.Id == demandeId && d.EtudiantId == etudiantId);

            if (demande == null)
                throw new Exception("Demande introuvable ou vous n'êtes pas autorisé à l'annuler");

            // Vérifier que la demande peut être annulée (seulement si elle est en attente)
            if (demande.Statut != StatutDemande.EnAttente)
                throw new Exception("Vous ne pouvez annuler que les demandes en attente");

            _context.DemandesColocation.Remove(demande);
            await _context.SaveChangesAsync();

            return true;
        }
        // 6. Méthode pour obtenir les demandes reçues (pour le propriétaire)
        public async Task<List<DemandesRecuesResponse>> ObtenirDemandesRecuesAsync(int proprietaireId)
        {
            var demandes = await _context.DemandesColocation
                .Include(d => d.Etudiant)
                .Include(d => d.Colocation)
                .Where(d => d.Colocation.EtudiantId == proprietaireId)
                .OrderByDescending(d => d.Id)
                .ToListAsync();

            return demandes.Select(d => new DemandesRecuesResponse
            {
                DemandeId = d.Id,
                NomEtudiant = $"{d.Etudiant.Prenom} {d.Etudiant.Nom}",
                EcoleEtudiant = d.Etudiant.Universite,
                Message = d.Message,
                Budget = d.Budget?.ToString("0") + " MAD/mois",
                DateEmmenagement = d.DateEmmenagement.ToString("yyyy-MM-dd"),
                Preferences = d.Preferences,
                Statut = EnumHelper.GetStatutFrancais(d.Statut),
                ColocationAdresse = d.Colocation.Adresse,
                ReponseEnvoyee = d.ReponseProprietaire,
                DateReponse = d.DateReponse?.ToString("yyyy-MM-dd")
            }).ToList();
        }

        // 7. Méthode pour filtrer les colocations
        public async Task<List<ColocationFiltreeResponse>> FiltrerColocationsAsync(FiltrerColocationsRequest request, int etudiantId)
        {
            // Récupérer toutes les colocations sauf celles de l'étudiant connecté
            var query = _context.Colocations
                .Include(c => c.Etudiant)
                .Where(c => c.EtudiantId != etudiantId); // Exclure ses propres colocations

            // Filtrer par budget maximum si spécifié
            if (request.BudgetMax.HasValue)
            {
                query = query.Where(c => c.Budget <= (double)request.BudgetMax.Value);
            }

        
            var colocations = await query.ToListAsync();

            // Filtrer par préférences si spécifiées
            if (request.Preferences != null && request.Preferences.Any())
            {
                colocations = colocations.Where(c =>
                    request.Preferences.Any(pref =>
                        c.Preferences.Any(cpref => cpref.ToLower().Contains(pref.ToLower()))
                    )
                ).ToList();
            }

            // Mapper vers le DTO de réponse
            return colocations.Select(c => new ColocationFiltreeResponse
            {
                Id = c.Id,
                Name = $"{c.Etudiant.Prenom} {c.Etudiant.Nom}",
                School = c.Etudiant.Universite,
                Budget = (decimal)c.Budget,
                MoveInDate = c.DateDebutDisponibilite.ToString("dd/MM/yyyy"),
                PreferredZone = c.Adresse,
                Type = "Offre", // Comme spécifié dans votre exemple
                Preferences = c.Preferences
            }).ToList();
        }
    }
}