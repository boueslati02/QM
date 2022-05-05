using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Common;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ponant.Medical.Board.Services
{
    /// <summary>
    /// Service d'authentification de l'application
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        #region AuthenticateUser
        /// <summary>
        /// Authentifie l'utilisateur sur l'application
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Instance de l'utilisateur connecté</returns>
        public Data.User AuthenticateUser(string username, string password)
        {
            Data.User user = null;

            using (BoardEntities db = new BoardEntities())
            {
                // Vérifie les credentials
                try
                {
                    user = db.User.Single(u => u.UserName.Equals(username));
                }
                catch
                {
                    throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");
                }

                // Gestion du blocage du compte
                if (!UserHelper.CheckPassword(user.PasswordHash, password))
                {
                    user.AccessFailedCount++;

                    if (user.AccessFailedCount == 3)
                    {
                        user.LockoutEndDate = DateTime.Now.AddMinutes(15);
                    }
                }
                else if (user.LockoutEndDate.HasValue && user.LockoutEndDate < DateTime.Now)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEndDate = null;
                }

                user.Editor = AppSettings.System;
                user.ModificationDate = DateTime.Now;
                db.SaveChanges();
                
                // Affichage des messages
                if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.Now)
                {
                    throw new UnauthorizedAccessException("Your account has been locked due to several unsuccessful attempts, please try again later. For further questions, please contact the IT department.");
                }
                else if (!UserHelper.CheckPassword(user.PasswordHash, password))
                {
                    throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");
                }
                
                return user;
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Modification du mot de passe
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="oldPassword">Nouveau mot de passe</param>
        /// <param name="newPassword">Nouveau mot de passe</param>
        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                // Modification à terre
                UserBoard userBoard = new UserBoard
                {
                    UserName = username,
                    PasswordHash = UserHelper.CalculateHash(newPassword)
                };

                await ShoreService.Instance.ChangePassword(userBoard);

                // Modification à bord
                using (BoardEntities db = new BoardEntities())
                {
                    Data.User user = db.User.Single(u => u.UserName.Equals(username));

                    if (user != null)
                    {
                        if (!UserHelper.CheckPassword(user.PasswordHash, oldPassword))
                        {
                            throw new MessageException("The old password is not correct.");
                        }

                        if (UserHelper.CheckPassword(user.PasswordHash, newPassword))
                        {
                            throw new MessageException("The new password must be different from the old one.");
                        }
                        
                        user.PasswordHash = userBoard.PasswordHash;
                        user.LockoutEndDate = null;
                        user.AccessFailedCount = 0;
                        user.ExpirationDate = DateTime.Now.AddMonths(3);
                        user.PasswordChange = false;
                        user.ModificationDate = DateTime.Now;
                        user.Editor = username;

                        db.SaveChanges();
                    }
                }
            }
            catch (MessageException exception)
            {
                throw new Exception(exception.Message);
            }
            catch
            {
                throw new Exception("The password could not be changed.");
            }
        }
        #endregion

        #region InitUser
        /// <summary>
        /// Initialisation des utilisateurs
        /// </summary>
        public static async void InitUser()
        {
            using (BoardEntities db = new BoardEntities())
            {
                if (!db.User.Any())
                {
                    Data.User localUser = new Data.User
                    {
                        AccessFailedCount = 0,
                        ExpirationDate = DateTime.Now.AddMonths(3),
                        PasswordChange = true,
                        Creator = AppSettings.System,
                        CreationDate = DateTime.Now,
                        Editor = AppSettings.System,
                        ModificationDate = DateTime.Now
                    };
                        
                    List<Medical.Data.Auth.User> users = await ShoreService.Instance.GetUsers();
                        
                    if (users != null)
                    {
                        foreach (Medical.Data.Auth.User user in users)
                        {
                            localUser.UserName = user.UserName;
                            localUser.IdShip = user.IdShip;
                            localUser.PasswordHash = user.PasswordHash;
                            db.User.Add(localUser);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Réinitialise les mots de passe en supprimant les lignes et en les recréant
        /// </summary>
        public void ResetPassword()
        {
            using (BoardEntities db = new BoardEntities())
            {
                foreach (Data.User user in db.User)
                {
                    db.User.Remove(user);
                }
                
                db.SaveChanges();
            }
            InitUser();
        }
        #endregion
    }

    #region MessageException
    /// <summary>
    /// Classe de gestion de l'exception du même mot de passe
    /// </summary>
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }
    }
    #endregion
}
