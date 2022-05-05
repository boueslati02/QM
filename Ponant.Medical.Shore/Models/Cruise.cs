namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #region Gestion des croisières
    /// <summary>
    /// Classe de gestion des croisière
    /// </summary>
    public class CruiseClass : SharedClass
    {
        #region Properties & Constructors

        public CruiseClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region UnlockCruise
        /// <summary>
        /// Débloque une croisière
        /// </summary>
        /// <param name="id">Identifiant de la croisire à débloquer</param>
        public void UnlockCruise(int id)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            try
            {
                List<Passenger> listPassengers = (from p in _shoreEntities.Passenger
                                                  join bcp in _shoreEntities.BookingCruisePassenger on p.Id equals bcp.IdPassenger
                                                  where bcp.IdCruise.Equals(id) && p.IsExtract
                                                  select p).Distinct().ToList();
                listPassengers.ForEach(p => { p.IsExtract = false; p.IdStatus = Constants.SHORE_STATUS_QM_RECEIVED; p.ModificationDate = Now; p.Editor = CurrentUser; });

                Cruise cruise = _shoreEntities.Cruise.Find(id);
                cruise.IsExtract = false;
                cruise.ModificationDate = Now;
                cruise.Editor = CurrentUser;

                _shoreEntities.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Cruise, LogManager.LogAction.Unlock, CurrentUser, "Unlock Cruise Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Cruise, LogManager.LogAction.Unlock, CurrentUser, "Unlock Cruise Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion
    }
    #endregion
}