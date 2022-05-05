using System;
using System.Collections.Generic;

namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Classe des mails reçus
    /// </summary>
    public class MailMessage
    {
        /// <summary>
        /// Identifiant du passager
        /// </summary>
        public int IdPassenger { get; set; }

        /// <summary>
        /// Identifiant du message
        /// </summary>
        public long IdMessage { get; set; }

        /// <summary>
        /// Corps du message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Adresse électronique de l'expéditeur
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Date de réception du mail
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Liste des pièces jointes enregistrées sur le serveur
        /// </summary>
        public List<MailAttachment> Attachments { get; set; }
    }
}
