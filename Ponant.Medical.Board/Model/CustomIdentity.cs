using System.Security.Principal;

namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion des utilisateurs authentifiés
    /// </summary>
    public class CustomIdentity : IIdentity
    {
        #region Properties
        public string Name { get; private set; }
        public string[] Roles { get; private set; }
        #endregion

        #region Constructors
        public CustomIdentity(string name, string[] roles)
        {
            Name = name;
            Roles = roles;
        }
        #endregion

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}
