namespace Ponant.Medical.Common.MailServer
{
    /// <summary>
    /// Classe des destinataires
    /// </summary>
    public class Recipient
    {
        /// <summary>
        /// Nom du destinataire
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adresse électronique du destinataire
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">Nom du destinataire</param>
        /// <param name="address">Adresse électronique du destinataire</param>
        public Recipient(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
