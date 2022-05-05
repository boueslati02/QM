namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions pour chaque passager traité (ligne du datagrid)
    /// </summary>
    public class PassengerDoneItemViewModel
    {
        #region Accessors

        /// <summary>
        /// Retourne/Positionne le nom du passager
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom usuel du passager
        /// </summary>
        public string UsualName { get; set; }

        /// <summary>
        /// Retourne/Positionne le prénom du passager
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Retourne/Positionne le prénom du passager
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Retourne/Positionne l'avis sur le passager
        /// </summary>
        public string Advice { get; set; }

        /// <summary>
        /// Retourne/Positionne les commentaires sur le passager
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Retourne/Positionne le statut du questionnaire associé au passager
        /// </summary>
        public string Status { get; set; }

        #endregion
    }
}