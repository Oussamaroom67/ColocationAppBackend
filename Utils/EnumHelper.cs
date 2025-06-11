using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Utils
{
    public static class EnumHelper
    {
        public static string GetStatutFrancais(StatutDemande statut)
        {
            return statut switch
            {
                StatutDemande.EnAttente => "En attente",
                StatutDemande.Acceptee => "Acceptée",
                StatutDemande.Refusee => "Refusée",
                _ => statut.ToString()
            };
        }

    }
}