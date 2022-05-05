namespace Ponant.Medical.Data.Auth
{
    /// <summary>
    /// Classe de retour des utilisateurs
    /// </summary>
    public class User
    {
        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mot de passe encrypté
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// L'identifiant du bateau
        /// </summary>
        public int? IdShip { get; set; }
    }
}
