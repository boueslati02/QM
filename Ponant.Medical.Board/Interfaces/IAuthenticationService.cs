using Ponant.Medical.Board.Data;
using System.Threading.Tasks;

namespace Ponant.Medical.Board.Interfaces
{
    /// <summary>
    /// Interface du service d'authentification
    /// </summary>
    public interface IAuthenticationService
    {
        User AuthenticateUser(string username, string password);
        Task ChangePassword(string username, string oldPassword, string newPassword);
        void ResetPassword();
    }
}
