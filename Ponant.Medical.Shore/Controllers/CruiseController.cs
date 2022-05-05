namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Web.Mvc;

    [Authorize(Roles = "Booking, Group, Booking Administrator, Medical Administrator, Medical, Doctor, IT Administrator, Agency Administrator, Agency")]
    public class CruiseController : BaseController
    {
        #region Properties & Construtors

        private readonly CruiseClass _cruiseClass;

        public CruiseController()
        {
            _cruiseClass = new CruiseClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des croisiére
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            LoadViewBag();
            return View();
        }
        #endregion

        #region Unlock
        /// <summary>
        /// Dévérouille une croisière
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Unlock(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    _cruiseClass.UnlockCruise(id.Value);
                    TempData["Message"] = "This cruise has been unlocking";
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "An error occurred while unlocking this cruise";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Private

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune
        /// </summary>
        private void LoadViewBag()
        {
            ViewBag.CruiseTypeList = _cruiseClass.GetLovList(Data.Constants.LOV_CRUISE_TYPE);
            ViewBag.DestinationList = _cruiseClass.GetLovList(Data.Constants.LOV_DESTINATION);
            ViewBag.ShipList = _cruiseClass.GetLovList(Data.Constants.LOV_SHIP);
        }
        #endregion

        #endregion
    }
}