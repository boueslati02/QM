namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Data.Entity.Validation;
    using System.Net;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator")]
    public class CriteriaController : BaseController
    {
        #region Properties & Constructors

        private readonly CriteriaClass _criteriaClass;

        public CriteriaController()
        {
            _criteriaClass = new CriteriaClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des critéres
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {         
            return View();
        }
        #endregion

        #region Create
        /// <summary>
        /// Formulaire de création d'un critére
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Create()
        {
            CreateCriteriaViewModel model = new CreateCriteriaViewModel();
            LoadViewBag();  
            return PartialView(model);
        }

        /// <summary>
        ///  Création d'un critére
        /// </summary>
        /// <param name="model">Instance du nouveau critére</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(CreateCriteriaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _criteriaClass.Create(model);
                    return Json(new { result = true, url = "/Criteria" });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError("", ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }

            }
            LoadViewBag();
            return PartialView(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification d'un critére
        /// <param name="id">Identifiant du critére</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EditCriteriaViewModel model = _criteriaClass.GetCriterion(id.Value);
            LoadViewBag();
            return PartialView(model);
        }

        /// <summary>
        ///  Modification d'un critére
        /// </summary>
        /// <param name="model">Instance du critére modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(EditCriteriaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _criteriaClass.Edit(model);
                    return Json(new { result = true, url = "/Criteria" });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError("", ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            LoadViewBag();
            return PartialView(model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un critére
        /// </summary>
        /// <param name="id">Identifiant du critére</param>
        /// <returns>Redirection vers la liste des critéres</returns>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    _criteriaClass.Delete(id.Value);
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting this criterion";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region CheckOrderValue
        /// <summary>
        /// Remote Validation Order
        /// </summary>
        /// <param name="order">Ordre à valider</param>
        /// <param name="id">Id du critére courrant</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckOrderValue(int order, int id)
        {
            bool result = _criteriaClass.IsValidOrder(order, id);
            if (!result)
            {
                if (id == 0)
                {
                    LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.Criteria, LogManager.LogAction.Add, User.Identity.Name, "Add criterion with existing order. Order : " + order.ToString());
                }
                else
                {
                    LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.Criteria, LogManager.LogAction.Edit, User.Identity.Name, "Edit criterion with existing order. Order : " + order.ToString() + " Id : " + id);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune entre _Create et _Edit
        /// </summary>
        private void LoadViewBag()
        {
            ViewBag.CruiseTypeList = _criteriaClass.GetLovList(Constants.LOV_CRUISE_TYPE);
            ViewBag.DestinationList = _criteriaClass.GetLovList(Constants.LOV_DESTINATION);
            ViewBag.ShipList = _criteriaClass.GetLovList(Constants.LOV_SHIP);
            ViewBag.ActivityList = _criteriaClass.GetLovList(Constants.LOV_ACTIVITY);
            ViewBag.SurveyList = _criteriaClass.GetSurveyList();
        }
        #endregion

        #endregion
    }
}