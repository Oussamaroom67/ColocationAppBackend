using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class DemandeLocationService
    {
        private readonly ApplicationDbContext _context;

        public DemandeLocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DemandeLocationResponseDto> EnvoyerDemandeAsync(DemandeLocationCreateDto demandeDto)
        {
            // Vérifier si l'annonce existe et est active
            var annonce = await _context.Annonces
                .Include(a => a.Logement)
                .FirstOrDefaultAsync(a => a.Id == demandeDto.AnnonceId);

            if (annonce == null)
                throw new ArgumentException("Annonce non trouvée");

            if (annonce.Statut != AnnonceStatus.Active)
                throw new InvalidOperationException("Cette annonce n'est plus disponible");

            // Vérifier si l'étudiant existe
            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Id == demandeDto.EtudiantId);

            if (etudiant == null)
                throw new ArgumentException("Étudiant non trouvé");

            // Vérifier si l'étudiant n'a pas déjà fait une demande pour cette annonce
            var demandeExistante = await _context.DemandesLocation
                .FirstOrDefaultAsync(d => d.AnnonceId == demandeDto.AnnonceId &&
                                         d.EtudiantId == demandeDto.EtudiantId);

            if (demandeExistante != null)
                throw new InvalidOperationException("Une demande a déjà été envoyée pour cette annonce");

            // Créer la nouvelle demande
            var nouvelleDemande = new DemandeLocation
            {
                AnnonceId = demandeDto.AnnonceId,
                EtudiantId = demandeDto.EtudiantId,
                Message = demandeDto.Message,
                DateEmmenagement = demandeDto.DateEmmenagement,
                DureeSejour = demandeDto.DureeSejour,
                NbOccupants = demandeDto.NbOccupants,
                status = LocationStatus.EnAttente,
                DateCreation = DateTime.Now
            };

            _context.DemandesLocation.Add(nouvelleDemande);
            await _context.SaveChangesAsync();

            // Charger la demande avec les relations pour la réponse
            var demandeComplete = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Etudiant)
                .FirstOrDefaultAsync(d => d.Id == nouvelleDemande.Id);

            return new DemandeLocationResponseDto
            {
                Id = demandeComplete.Id,
                AnnonceId = demandeComplete.AnnonceId,
                TitreAnnonce = demandeComplete.Annonce.Titre,
                EtudiantId = demandeComplete.EtudiantId,
                NomEtudiant = $"{demandeComplete.Etudiant.Nom} {demandeComplete.Etudiant.Prenom}",
                Message = demandeComplete.Message,
                DateEmmenagement = demandeComplete.DateEmmenagement,
                DureeSejour = demandeComplete.DureeSejour,
                NbOccupants = demandeComplete.NbOccupants,
                Status = demandeComplete.status,
                MessageReponse = demandeComplete.MessageReponse,
                DateReponse = demandeComplete.DateReponse != DateTime.MinValue ? demandeComplete.DateReponse : null,
                DateCreation = demandeComplete.DateCreation
            };
        }

        public async Task<DemandeLocationResponseDto> ChangerStatusAsync(ChangeStatusDto changeStatusDto, int proprietaireId)
        {
            var demande = await _context.DemandesLocation
                .Include(d => d.Annonce)
                    .ThenInclude(a => a.Logement)
                        .ThenInclude(l => l.Proprietaire)
                .Include(d => d.Etudiant)
                .FirstOrDefaultAsync(d => d.Id == changeStatusDto.DemandeId);

            if (demande == null)
                throw new ArgumentException("Demande non trouvée");

            // Vérifier que l'utilisateur est le propriétaire de l'annonce
            if (demande.Annonce.Logement.Proprietaire.Id != proprietaireId)
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à traiter cette demande");

            if (demande.status != LocationStatus.EnAttente)
                throw new InvalidOperationException("Cette demande a déjà été traitée");

            // Valider le nouveau statut
            if (changeStatusDto.NouveauStatus == LocationStatus.EnAttente)
                throw new ArgumentException("Le statut ne peut pas être remis en attente");

            // Mettre à jour la demande
            demande.status = changeStatusDto.NouveauStatus;
            demande.MessageReponse = changeStatusDto.MessageReponse;
            demande.DateReponse = DateTime.Now;

            await _context.SaveChangesAsync();

            return new DemandeLocationResponseDto
            {
                Id = demande.Id,
                AnnonceId = demande.AnnonceId,
                TitreAnnonce = demande.Annonce.Titre,
                EtudiantId = demande.EtudiantId,
                NomEtudiant = $"{demande.Etudiant.Nom} {demande.Etudiant.Prenom}",
                Message = demande.Message,
                DateEmmenagement = demande.DateEmmenagement,
                DureeSejour = demande.DureeSejour,
                NbOccupants = demande.NbOccupants,
                Status = demande.status,
                MessageReponse = demande.MessageReponse,
                DateReponse = demande.DateReponse,
                DateCreation = demande.DateCreation
            };
        }

        public async Task<List<DemandeLocationRecusProprietaireResponseDto>> GetAllReceivedRequestsLocation(int proprietaireId)
        {
            var demandes = await _context.DemandesLocation
             .Include(d => d.Annonce)
                 .ThenInclude(a => a.Logement)
                     .ThenInclude(l => l.Proprietaire)
             .Include(d => d.Etudiant)
             .Where(d => d.Annonce.Logement.Proprietaire.Id == proprietaireId)
             .OrderByDescending(d => d.DateCreation)
             .ToListAsync();
            return demandes.Select(d => new DemandeLocationRecusProprietaireResponseDto
            {
                Id = d.Id,
                Nom = $"{d.Etudiant.Nom} {d.Etudiant.Prenom}",
                Email = d.Etudiant.Email,
                Logement = d.Annonce.Titre,
                DateDemande = d.DateCreation.ToString("dd MMMM yyyy"),
                Budget = d.Annonce.Prix.ToString("C"),
                Message = d.Message ?? "",
                Universite = d.Etudiant.Universite,
                Annee = d.Etudiant.NiveauEtudes,
                Duree = d.DureeSejour switch
                {
                    1 => "1 mois",
                    < 12 => $"{d.DureeSejour} mois",
                    12 => "1 an",
                    _ => $"{d.DureeSejour / 12} an(s)"
                },
                Emmenagement = d.DateEmmenagement.ToString("dd MMMM yyyy"),
                Status = d.status switch
                {
                    LocationStatus.EnAttente => "en-attente",
                    LocationStatus.Accepté => "approuvee",
                    LocationStatus.Refusée => "rejetee",
                    _ => "en-attente"
                },
                Priority = GetPriority(d.DateCreation, d.status),
                AnnonceId = d.AnnonceId,
                EtudiantId = d.EtudiantId,
                MessageReponse = d.MessageReponse,
                DateReponse = d.DateReponse,
                DateCreation = d.DateCreation,
                DateEmmenagementOriginal = d.DateEmmenagement,
                DureeSejour = d.DureeSejour,
                NbOccupants = d.NbOccupants,
                StatusOriginal = d.status
            }).ToList();
        }

            
        private string GetPriority(DateTime dateCreation, LocationStatus status)
        {
            if (status != LocationStatus.EnAttente) return "moyenne";

            var daysSinceCreation = (DateTime.Now - dateCreation).TotalDays;

            return daysSinceCreation switch
            {
                > 7 => "haute",
                > 3 => "moyenne",
                _ => "basse"
            };
        }
        public async Task<List<DemandeLocationResponseDto>> GetDemandesParEtudiantAsync(int etudiantId)
        {
            var demandes = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Etudiant)
                .Where(d => d.EtudiantId == etudiantId)
                .OrderByDescending(d => d.DateCreation)
                .ToListAsync();

            return demandes.Select(d => new DemandeLocationResponseDto
            {
                Id = d.Id,
                AnnonceId = d.AnnonceId,
                TitreAnnonce = d.Annonce.Titre,
                EtudiantId = d.EtudiantId,
                NomEtudiant = $"{d.Etudiant.Nom} {d.Etudiant.Prenom}",
                Message = d.Message,
                DateEmmenagement = d.DateEmmenagement,
                DureeSejour = d.DureeSejour,
                NbOccupants = d.NbOccupants,
                Status = d.status,
                MessageReponse = d.MessageReponse,
                DateReponse = d.DateReponse != DateTime.MinValue ? d.DateReponse : null,
                DateCreation = d.DateCreation
            }).ToList();
        }
    }
}

