using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.Services
{
    /// <summary>
    /// Service d'appel de la Web Api sur la terre
    /// </summary>
    public sealed class ShoreService
    {
        #region Properties
        /// <summary>
        /// Instance unique de cette classe
        /// </summary>
        private static ShoreService instance = new ShoreService();

        /// <summary>
        /// Instance du client HTTP
        /// </summary>
        private HttpClient client = new HttpClient();

        /// <summary>
        /// Retourne l'instance unique de cette classe (Singleton)
        /// </summary>
        public static ShoreService Instance
        {
            get
            {
                return NetworkService.Instance.IsNetworkAvailable() ? instance : null;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        private ShoreService()
        {
            client.BaseAddress = new Uri(AppSettings.WebServiceUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            byte[] byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Application.Current.Properties["UserName"], Application.Current.Properties["Password"]));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

#if DEV || INTEGRATION
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
#endif
        }
        #endregion

        #region Public methods

        #region ChangePassword
        /// <summary>
        /// Modifier le mot de passe de l'utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="passwordHash">Mot de passe encrypté</param>
        /// <returns></returns>
        public async Task<bool> ChangePassword(UserBoard user)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", AppSettings.ShoreUserName, AppSettings.ShorePassword));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/user", user);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "ChangePassword", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region ChangeStatusPassenger
        /// <summary>
        /// Modifie le statut du passager sur la terre
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <returns></returns>
        public async Task<bool> ChangeStatusPassenger(int idPassenger)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                HttpResponseMessage response = await client.PutAsync(string.Format("api/passenger/changestatuspassenger?id={0}", idPassenger), null);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "ChangeStatusPassenger", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region EditAdvice
        /// <summary>
        /// Met à jour le statut du passager sur la terre
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="advice">Instance de l'avis du médecin</param>
        /// <returns></returns>
        public async Task<bool> EditAdvice(AdviceBoard advice)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync("api/passenger/editadvice?id=" + advice.IdPassenger, advice);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "EditAdvice", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region ExtractPassenger
        /// <summary>
        /// Extrait le passager sur la terre
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns></returns>
        public async Task<bool> ExtractPassenger(int idPassenger, int idCruise)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                HttpResponseMessage response = await client.PutAsync(string.Format("api/passenger/extractpassenger?id={0}&idCruise={1}", idPassenger, idCruise), null);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "ExtractPassenger", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region FreeCruise
        /// <summary>
        /// Libère la croisière et ses passagers
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns></returns>
        public async Task<bool> FreeCruise(int idCruise)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                HttpResponseMessage response = await client.PutAsync(string.Format("api/cruise/freecruise?id={0}", idCruise), null);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "FreeCruise", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region GetCruises
        /// <summary>
        /// Télécharge les croisières à partir du web service
        /// </summary>
        /// <returns></returns>
        public async Task<List<vCruiseBoard>> GetCruises()
        {
            List<vCruiseBoard> cruises = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/cruise");
                if (response.IsSuccessStatusCode)
                {
                    cruises = await response.Content.ReadAsAsync<List<vCruiseBoard>>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "GetCruises", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return cruises;
        }
        #endregion

        #region GetLov
        /// <summary>
        /// Recupére les Lov 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Lov>> GetLov()
        {
            List<Lov> lovs = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/lov");
                if (response.IsSuccessStatusCode)
                {
                    lovs = await response.Content.ReadAsAsync<List<Lov>>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "GetLov", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return lovs;
        }
        #endregion

        #region GetPassengersByCruise
        /// <summary>
        /// Récupère la liste des identifiants des passagers pour une croisière donnée
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idStatus">Identifiant du statut du QM</param>
        /// <param name="forCurrentCruises">Indique que la récupération des passagers se fait pour les croisières à venir</param>
        /// <returns></returns>
        public async Task<List<PassengerCruise>> GetPassengersByCruise(int idCruise, int? idStatus, bool forCurrentCruises = false)
        {
            List<PassengerCruise> passengersCruises = null;

            try
            {
                string request = string.Format("api/passenger/getpassengersbycruise?id={0}&status={1}&forCurrentCruises={2}", idCruise, idStatus, forCurrentCruises);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    passengersCruises = await response.Content.ReadAsAsync<List<PassengerCruise>>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "GetCruises", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return passengersCruises;
        }
        #endregion

        #region GetPassenger
        /// <summary>
        /// Récupère les informations d'un passager
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <returns></returns>
        public async Task<vPassengerBoard> GetPassenger(int idPassenger, int idCruise)
        {
            vPassengerBoard passenger = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(string.Format("api/passenger?id={0}&idCruise={1}", idPassenger, idCruise));
                if (response.IsSuccessStatusCode)
                {
                    passenger = await response.Content.ReadAsAsync<vPassengerBoard>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "GetPassenger", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return passenger;
        }
        #endregion

        #region GetUsers
        /// <summary>
        /// Récupère les utilisateurs des navires
        /// </summary>
        /// <returns></returns>
        public async Task<List<Medical.Data.Auth.User>> GetUsers()
        {
            List< Medical.Data.Auth.User> users = null;

            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", AppSettings.ShoreUserName, AppSettings.ShorePassword));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await client.GetAsync("api/user");
                
                if (response.IsSuccessStatusCode)
                {
                    users = await response.Content.ReadAsAsync<List< Medical.Data.Auth.User>>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "GetUsers", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return users;
        }
        #endregion

        #region IsExtractCruise
        /// <summary>
        /// Vérifie que la croisière est toujours extraite
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns>Vrai si la croisière est extraite, faux sinon</returns>
        public async Task<bool?> IsExtractCruise(int idCruise)
        {
            bool? isExtract = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/cruise/isextractcruise?id=" + idCruise);
                if (response.IsSuccessStatusCode)
                {
                    isExtract = await response.Content.ReadAsAsync<bool>();
                }
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "IsExtractCruise", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return isExtract;
        }
        #endregion

        #region UnlinkDocument
        /// <summary>
        /// Détache le document du passager
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns></returns>
        public async Task<bool> UnlinkDocument(int idDocument)
        {
            bool IsSuccessStatusCode = false;

            try
            {
                HttpResponseMessage response = await client.PutAsync("api/document/" + idDocument, null);
                IsSuccessStatusCode = response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                Logger.Log("ShoreService", "UnlinkDocument", exception);
#if DEV || INTEGRATION
                throw;
#endif
            }

            return IsSuccessStatusCode;
        }
        #endregion

        #region UpdateHeaders
        /// <summary>
        /// Réinitialise les entêtes lors d'un changement de mot de passe
        /// </summary>
        public void UpdateHeaders()
        {
            instance = new ShoreService();
        }
        #endregion

        #endregion
    }
}