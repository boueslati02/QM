namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Classe des pièces jointes
    /// </summary>
    public class MailAttachment
    {
        /// <summary>
        /// Nom du fichier initial
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nom du fichier renommé
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Identifiant du passager attaché au document
        /// </summary>
        public int? IdPassenger { get; set; }
    }
}
