using Ponant.Medical.Common;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Ponant.Medical.WebServices.Controllers
{
    /// <summary>
    /// Gestion des ressources de croisières
    /// </summary>
    [Authorize]
    public class CruiseController : ApiController
    {
        #region Properties & Constructors

        private readonly IShoreEntities _shoreEntities;

        /// <summary>
        /// Document Controller
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        public CruiseController(IShoreEntities shoreEntities)
        {
            _shoreEntities = shoreEntities;
        }

        #endregion

        #region FreeCruise
        /// <summary>
        /// Libère la croisière ainsi que tous ses passagers
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <returns>Ok si la croisière a bien été libérée</returns>
        [HttpPut]
        [Route("api/cruise/freecruise")]
        public IHttpActionResult FreeCruise(int id)
        {
            Cruise cruise = null;
            try
            {
                // Libération de la croisière
                try
                {
                    cruise = _shoreEntities.Cruise.Single(c => c.Id.Equals(id));
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }

                cruise.IsExtract = false;
                cruise.Editor = HttpContext.Current.User.Identity.Name;
                cruise.ModificationDate = DateTime.Now;

                // Libération des passagers
                foreach (BookingCruisePassenger bookingCruisePassenger in cruise.BookingCruisePassenger)
                {
                    if (bookingCruisePassenger.Passenger.IdStatus.Equals(Constants.SHORE_STATUS_QM_IN_PROGRESS))
                    {
                        bookingCruisePassenger.Passenger.IdStatus = Constants.SHORE_STATUS_QM_RECEIVED;
                        bookingCruisePassenger.Passenger.IsExtract = false;
                        bookingCruisePassenger.Passenger.Editor = HttpContext.Current.User.Identity.Name;
                        bookingCruisePassenger.Passenger.ModificationDate = DateTime.Now;
                    }
                }

                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Cruise, LogManager.LogAction.Unlock, HttpContext.Current.User.Identity.Name, "Free cruise : " + cruise.Code);
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Cruise, LogManager.LogAction.Unlock, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Ok();
        }
        #endregion

        #region GetCruises
        /// <summary>
        /// Retourne la liste des croisières à traiter
        /// </summary>
        /// <returns>Liste des croisières à traiter</returns>
        [HttpGet]
        public IHttpActionResult GetCruises()
        {
            List<vCruiseBoard> cruises;

            try
            {
                cruises = _shoreEntities.vCruiseBoard.ToList();
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Cruise, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Json(cruises);
        }
        #endregion

        #region IsExtractCruise
        /// <summary>
        /// Vérifie que la croisière est toujours bloquée
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <returns>Vrai si la croisière est bloqué, faux sinon</returns>
        [HttpGet]
        [Route("api/cruise/isextractcruise")]
        public IHttpActionResult IsExtractCruise(int id)
        {
            Cruise cruise = null;
            try
            {
                cruise = _shoreEntities.Cruise.Single(c => c.Id.Equals(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return Json(cruise.IsExtract);
        }
        #endregion
    }
}
