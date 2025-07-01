using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses.AnalyticsDTOs;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class AnalytiquesService
    {
        private readonly ApplicationDbContext _context;

        public AnalytiquesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AnalyticsDTO> GetAnalyticsAsync()
        {
            // Récupération des données de base depuis la base de données
            var reservations = await _context.DemandesLocation.ToListAsync();
            var annonces = await _context.Annonces.ToListAsync();

            // Calcul du taux d'occupation global
            var totalLogements = await _context.Logements.CountAsync();
            var logementsOccupes = annonces.Count(l => l.Statut == AnnonceStatus.Louee);
            var tauxOccupation = totalLogements == 0 ? 0 : (double)logementsOccupes / totalLogements * 100;

            // Comptage des étudiants et propriétaires
            var totalEtudiants = await _context.Etudiants.CountAsync();
            var totalProprietaires = await _context.Proprietaires.CountAsync();
            var totalUtilisateurs = totalEtudiants + totalProprietaires;

            // Nombre total de réservations et calcul du loyer moyen
            var totalReservations = await _context.DemandesLocation.CountAsync();
            var prixLoyerMoyen = annonces.Count() != 0 ? annonces.Average(l => l.Prix) : 0;

            // Évolution des réservations sur les 6 derniers mois
            var now = DateTime.UtcNow;
            var reservationsMois = Enumerable.Range(0, 6).Select(i =>
            {
                var mois = now.AddMonths(-i);
                var debut = new DateTime(mois.Year, mois.Month, 1);
                var fin = debut.AddMonths(1);
                var nombre = reservations.Count(r => r.DateCreation >= debut && r.DateCreation < fin);
                var occupation = totalLogements == 0 ? 0 : (double)nombre / totalLogements * 100;

                return new ReservationMoisDto
                {
                    Mois = debut.ToString("MMM"),
                    NombreReservations = nombre,
                    TauxOccupation = occupation
                };
            }).Reverse().ToList();

            // Calcul du pourcentage d'évolution par rapport au mois précédent
            var lastMonthCount = reservationsMois[4].NombreReservations;
            var thisMonthCount = reservationsMois[5].NombreReservations;
            var evolutionPourcentage = lastMonthCount == 0 ? 0 : ((double)(thisMonthCount - lastMonthCount) / lastMonthCount) * 100;

            // Regrouper les logements par type
            var repartitionParType =  await _context.Logements.GroupBy(l => l.Type).Select(g => new TypeLogementDto
            {
                Type = g.Key,
                Count = g.Count()
            }).ToListAsync();

            // Regrouper les logements par ville avec calculs statistiques
            var repartitionParVille = await _context.Annonces
                .GroupBy(l => l.Logement.Ville)
                .Select(g => new VilleStatDto
                {
                    Ville = g.Key,
                    NombreLogements = g.Count(),
                    PrixMoyen = (double)g.Average(l => l.Prix),
                    TauxOccupation = g.Count() == 0 ? 0 : (double)g.Count(l => l.Statut== AnnonceStatus.Louee) / g.Count() * 100
                }).Take(5).ToListAsync();

            //Verfications des logements
            var verifies = annonces.Count(r => r.StatutVerification == VerificationLogementStatut.Verifie);
            var enAttente = annonces.Count(r => r.StatutVerification == VerificationLogementStatut.EnAttente);
            var rejetés = annonces.Count(r => r.StatutVerification == VerificationLogementStatut.Rejetée);

            var TauxVerification = annonces.Count() == 0 ? 0 : (double)verifies / annonces.Count() * 100;

            //repartition signalement selon les motif 
            var repartitionSignalements = await _context.Signalments
            .GroupBy(s => s.Motif) 
            .Select(g => new SignalementMotifDto
            {
                Motif = g.Key.ToString(),
                Nombre = g.Count()
            }).ToListAsync();
            //repartition etudiants selon les Etablissements
            var repartitionParEtablissement =await _context.Etudiants.GroupBy(l => l.Universite).Select(g => new EtablissementStudentsDto
            {
                Etablissement = g.Key,
                NombreEtudiants = g.Count()
            }).ToListAsync();

            //partie Recherche Colocataire
            var demandesActives = await _context.DemandesColocation.CountAsync();
            var matchReussis = await _context.DemandesColocation.CountAsync(r => r.Statut == StatutDemande.Acceptee);
            var TauxReussite = demandesActives == 0 ? 0 : matchReussis / demandesActives *100;

            //Repartition etudiants par budgets 

            var repartitionBudget = new List<BudgetEtudiantDto>
            {
                new BudgetEtudiantDto
                {
                    Tranche = "< 400DH",
                    NombreEtudiants = _context.Etudiants.Count(e => e.Budget < 400),
                },
                new BudgetEtudiantDto
                {
                    Tranche = "400DH - 600DH",
                    NombreEtudiants = _context.Etudiants.Count(e => e.Budget >= 400 && e.Budget <= 600),
                },
                new BudgetEtudiantDto
                {
                    Tranche = "600DH - 800DH",
                    NombreEtudiants =_context.Etudiants.Count(e => e.Budget > 600 && e.Budget <= 800),
                },
                new BudgetEtudiantDto
                {
                    Tranche = "> 800DH",
                    NombreEtudiants = _context.Etudiants.Count(e => e.Budget > 800),
                }
            };

            // Calcul du pourcentage
            foreach (var item in repartitionBudget)
            {
                item.Pourcentage = totalEtudiants == 0 ? 0 : (double)item.NombreEtudiants / totalEtudiants * 100;
            }


            // Retour de l'objet contenant toutes les statistiques
            return new AnalyticsDTO
            {
                TauxOccupation = tauxOccupation,
                TotalUtilisateurs = totalUtilisateurs,
                TotalEtudiants = totalEtudiants,
                TotalProprietaires = totalProprietaires,
                TotalReservations = totalReservations,
                EvolutionReservationsPourcentage = evolutionPourcentage,
                PrixLoyerMoyen = (double)prixLoyerMoyen,
                EvolutionReservations = reservationsMois,
                RepartitionParType = repartitionParType,
                RepartitionParVille = repartitionParVille,
                LogementVerifies = verifies,
                LogementAttentes = enAttente,
                Rejetes = rejetés,
                TauxVerification = TauxVerification,
                RepartitionSignalementsParMotif = repartitionSignalements,
                RepartitionEtudiantsParEtablissements = repartitionParEtablissement,
                DemandeActives = demandesActives,
                MatchReussis = matchReussis,
                TauxDeReussite = TauxReussite,
                RepartitionBudgetEtudiants = repartitionBudget
            };
        }
    }
}
