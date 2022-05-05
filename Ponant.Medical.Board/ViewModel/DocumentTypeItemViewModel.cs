namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Elément de types de document
    /// </summary>
    public class DocumentTypeItemViewModel
    {
        #region Properties

        /// <summary>
        /// Retourne/Positionne l'Id du type de document
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom du type de document
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type de document
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Retourne/Positionne l'état de sélection du type de document
        /// </summary>
        public bool IsChecked { get; set; }

        #endregion
    }
}