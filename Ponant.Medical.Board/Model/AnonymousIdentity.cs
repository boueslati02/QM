namespace Ponant.Medical.Board.Model
{
    /// <summary>
    /// Classe de gestion des utilisateurs qui ne sont pas encore authentifiés
    /// </summary>
    public class AnonymousIdentity : CustomIdentity
    {
        #region Constructors
        public AnonymousIdentity()
        : base(string.Empty, new string[] { })
        { }
        #endregion
    }
}
