using Ponant.Medical.Common;
using Ponant.Medical.Data.Auth;
using Ponant.Medical.Data.Auth.Interfaces;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Ponant.Medical.WebServices.Controllers
{
    /// <summary>
    /// Gestion des utilisateurs des navires
    /// </summary>
    [Authorize]
    public class UserController : ApiController
    {
        #region Properties & Constructors

        private readonly IAuthContext _authContext;

        /// <summary>
        /// User Controller
        /// </summary>
        /// <param name="authContext">Auth Context</param>
        public UserController(IAuthContext authContext)
        {
            _authContext = authContext;
        }

        #endregion

        #region GetUsers
        /// <summary>
        /// Récupération de la liste des utilisateurs des navires
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            List<User> users = null;

            try
            {
                users = _authContext.AspNetUsers
                    .Where(u => u.AspNetRoles.Any(r => r.Name.Equals(AppSettings.RoleBoard)))
                    .Select(u => new User { UserName = u.UserName, PasswordHash = u.PasswordHashInit, IdShip = u.IdShip })
                    .ToList();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, "Récupération de la liste des utilisateurs des navires");
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.User, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Json(users);
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Met à jour le mot de passe de l'utilisateur
        /// </summary>
        /// <param name="userBoard">Instance de l'utilisateur bord</param>
        /// <returns>Ok si la mise s'est bien passé</returns>
        [HttpPut]
        public IHttpActionResult ChangePassword(UserBoard userBoard)
        {
            if (userBoard == null)
            {
                return BadRequest("UserBoard not defined.");
            }

            try
            {
                AspNetUsers user = _authContext.AspNetUsers.Single(u => u.UserName.Equals(userBoard.UserName));
                user.PasswordHash = userBoard.PasswordHash;
                user.ModificationDate = DateTime.Now;
                user.Editor = HttpContext.Current.User.Identity.Name;
                _authContext.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.User, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Mise à jour du mot de passe de " + userBoard.UserName);
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.User, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Ok();
        }
        #endregion
    }
}