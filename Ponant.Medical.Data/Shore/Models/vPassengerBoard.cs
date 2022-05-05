using System.Collections.Generic;

namespace Ponant.Medical.Data.Shore
{
    /// <summary>
    /// Enrichissement de la vue passager pour les services web
    /// </summary>
    public partial class vPassengerBoard
    {
        /// <summary>
        /// Liste des identifiants d'informations complémentaires à l'avis du médecin
        /// </summary>
        public List<int> Informations { get; set; }

        /// <summary>
        /// Flux binaire du fichier zip des documents du passagers
        /// </summary>
        public byte[] Documents { get; set; }

        /// <summary>
        /// Liste des identifiants et nom de documents
        /// </summary>
        public Dictionary<int, string> DocumentsInfos { get; set; }
    }
}
