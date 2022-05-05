using System.Collections.Generic;

namespace Ponant.Medical.Data.Shore
{
    /// <summary>
    /// Modèle d'envoi des avis des médecins pour un passager
    /// </summary>
    public class AdviceBoard
    {
        /// <summary>
        /// Identifiant du passager
        /// </summary>
        public int IdPassenger { get; set; }

        /// <summary>
        /// Identifiant de la croisière
        /// </summary>
        public int IdCruise { get; set; }

        /// <summary>
        /// Identifiant de l'avis
        /// </summary>
        public int IdAdvice { get; set; }

        /// <summary>
        /// Commentaires complémentaires
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Liste des identifiants d'informations complémentaires à l'avis du médecin
        /// </summary>
        public List<int> Informations { get; set; }
    }
}
