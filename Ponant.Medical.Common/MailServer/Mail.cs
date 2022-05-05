using System.Collections.Generic;

namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Classe des courriers
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// Liste des pièces jointes
        /// </summary>
        public List<string> Attachments { get; set; }

        /// <summary>
        /// Corps du message
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Nom de l'expéditeur
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Adresse mail de l'expéditeur
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Objet du message
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Liste des destinataires
        /// </summary>
        public List<Recipient> Recipients { get; set; }
    }
}
