using System.ComponentModel;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Instance d'un avis de médecin
    /// </summary>
    public class AdviceItemViewModel
    {
        #region Properties

        /// <summary>
        /// Retourne/Positionne l'Id de l'avis
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Retourne/Positionne le texte de l'avis
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Retourne/Positionne la liste des types de documents associés à l'avis
        /// </summary>
        public ICollectionView DocumentTypeItemsView { get; set; }

        #endregion
    }
}
