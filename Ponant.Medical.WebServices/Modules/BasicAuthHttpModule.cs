using Newtonsoft.Json;
using Ponant.Medical.Common;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Auth;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace Ponant.Medical.WebServices.Modules
{
    /// <summary>
    /// Gestion de l'authentification en mode basic
    /// </summary>
    /// <see cref="https://www.asp.net/web-api/overview/security/basic-authentication"/>
    public class BasicAuthHttpModule : IHttpModule
    {
        private const string Realm = "Ponant";

        /// <summary>
        /// Initialisation des évènements
        /// </summary>
        /// <param name="context">Contexte de l'application</param>
        public void Init(HttpApplication context)
        {
            // Register event handlers
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        /// <summary>
        /// Définition du contexte d'authentification
        /// </summary>
        /// <param name="principal">Instance du context d'authentification</param>
        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        /// <summary>
        /// Vérifie la validité des informations de connexion
        /// </summary>
        /// <param name="username">Nom de l'utilisateur applicatif</param>
        /// <param name="password">Mot de passe applicatif</param>
        /// <returns>Vrai si les infos sont validées</returns>
        private static bool CheckPassword(string username, string password)
        {
            bool isChecked = false;

            try
            {
                using (AuthContext db = new AuthContext())
                {
                    AspNetUsers user = db.AspNetUsers.SingleOrDefault(u => u.UserName.Equals(username) && u.AspNetRoles.Any(r => r.Name == Constants.ROLE_NAME_BOARD));

                    if (user != null)
                    {
                        isChecked = UserHelper.CheckPassword(user.PasswordHash, password);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Connection, LogManager.LogAction.Get, username, exception);
#if DEV || INTEGRATION
                throw new Exception("BasicAuthHttpModule.CheckPassword", exception);
#endif
            }

            return isChecked;
        }

        /// <summary>
        /// Authentifie l'utilisateur
        /// </summary>
        /// <param name="credentials">Valeur des credentials passées dans l'entête</param>
        private static void AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                if (CheckPassword(name, password))
                {
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        /// <summary>
        /// Vérification de l'autorisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        /// <summary>
        /// Si la requête n'est pas autorisé, ajout de l'entête WWW-Authenticate à l'entête de réponse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
                response.ContentType = "application/json; charset=utf-8";
                response.ClearContent();

                using (var reader = new StreamWriter(response.OutputStream))
                {
                    JsonSerializer js = new JsonSerializer();
                    js.Serialize(reader, new { Message = "L’autorisation a été refusée pour cette demande." });
                }
            }
        }

        public void Dispose()
        {
        }
    }
}