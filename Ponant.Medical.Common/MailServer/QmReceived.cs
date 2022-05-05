using System;

namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Qm received
    /// </summary>
    public class QmReceived
    {
        #region Properties
        /// <summary>
        /// Numéro du Booking
        /// </summary>
        public int BookingNumber { get; set; }

        /// <summary>
        /// Prénom du passager
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom du passager
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Nom usuel du passager
        /// </summary>
        public string UsualName { get; set; }

        /// <summary>
        /// Date de réception du QM
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// Numéro de la croisière
        /// </summary>
        public string CruiseNumber { get; set; }
        #endregion
    }
}
