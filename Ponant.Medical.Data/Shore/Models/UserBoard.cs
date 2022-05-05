namespace Ponant.Medical.Data.Shore
{
    public class UserBoard
    {
        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mot de passe haché
        /// </summary>
        public string PasswordHash { get; set; }
    }
}
