using System.Linq;
using System.Security.Principal;

namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion du contexte de sécurité de l'application
    /// </summary>
    public class CustomPrincipal : IPrincipal
    {
        #region Properties
        private CustomIdentity _identity;

        public CustomIdentity Identity
        {
            get { return _identity ?? new AnonymousIdentity(); }
            set { _identity = value; }
        }
        #endregion

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public bool IsInRole(string role)
        {
            return _identity.Roles.Contains(role);
        }
        #endregion
    }
}