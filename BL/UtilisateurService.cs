using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
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
                Statut = IsUserSuspended(u) ? "Suspendu" : "Actif",
                Verifie = u.EstVerifie ? "Vérifié" : "Non vérifié",
                DateInscription = u.DateInscription,
                NbrUtilisateur = utilisateurs.Count(),
                NbrProprietaire = utilisateurs.Count(r => r is Proprietaire),
                NbrEtudiants = utilisateurs.Count(r => r is Etudiant),
                NbrAdministrateurs = utilisateurs.Count(r => r is Administrateur),
            }).ToList();

            return result;
        }
        //bannir User
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(r => r.Id == id);
                if (user == null)
                    return false;

                // Supprimer tous les messages
                var messages = await _context.Messages.Where(r => r.ExpediteurId == id).ToListAsync();
                _context.Messages.RemoveRange(messages);

                // Supprimer toutes les conversations où il est utilisateur1 ou utilisateur2
                var conversations1 = await _context.Conversations.Where(r => r.Utilisateur1Id == id).ToListAsync();
                var conversations2 = await _context.Conversations.Where(r => r.Utilisateur2Id == id).ToListAsync();
                _context.Conversations.RemoveRange(conversations1);
                _context.Conversations.RemoveRange(conversations2);

                // Supprimer tous les signalements liés à cet utilisateur
                var signalements = await _context.Signalments
                    .Where(r => r.UtilisateurSignaleId == id || r.SignaleurId == id || r.ResoluParId == id)
                    .ToListAsync();
                _context.Signalments.RemoveRange(signalements);

                // Supprimer tous les signalements liés à cet utilisateur
                var avis = await _context.Avis
                    .Where(r => r.StudentId == id || r.ProprietaireId == id)
                    .ToListAsync();
                _context.Avis.RemoveRange(avis);

                var colocations = await _context.Colocations.Where(c => c.EtudiantId == id).ToListAsync();
                foreach (var coloc in colocations)
                {
                    var demandes = await _context.DemandesColocation
                        .Where(d => d.ColocationId == coloc.Id)
                        .ToListAsync();

                    _context.DemandesColocation.RemoveRange(demandes);
                }

                // Ensuite supprimer les colocations
                _context.Colocations.RemoveRange(colocations);

                // Enfin, supprimer l'utilisateur
                _context.Utilisateurs.Remove(user);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression : " + ex.Message, ex);
            }
        }

        //suspendre user
        public async Task<bool> SuspendreUser(int id, bool suspendre)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(r => r.Id == id);
            if (user == null) return false;

            if (suspendre)
            {
                user.LastSuspendedAt = DateTime.UtcNow;
            }
            else
            {
                user.LastSuspendedAt = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public bool IsUserSuspended(Utilisateur user)
        {
            if (user.LastSuspendedAt == null)
                return false;

            return user.LastSuspendedAt.Value.AddDays(7) > DateTime.UtcNow;
        }


    }
}
