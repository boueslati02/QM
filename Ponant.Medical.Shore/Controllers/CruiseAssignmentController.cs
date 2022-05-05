using Ponant.Medical.Shore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Ponant.Medical.Shore.Models.CruiseAssignmentClass;

namespace Ponant.Medical.Shore.Controllers
{
    [Authorize(Roles = "Medical Administrator")]
    public class CruiseAssignmentController : BaseController
    {
        #region Properties & Construtors

        private readonly CruiseClass _cruiseClass;
        private readonly CruiseAssignmentClass _cruiseAssignmentClass;

        public CruiseAssignmentController()
        {
            _cruiseClass = new CruiseClass(_shoreEntities);
            _cruiseAssignmentClass = new CruiseAssignmentClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des droits d'accès et les filtres
        /// </summary>
        /// <returns>Retourne la vue</returns>
        [HttpGet]
        public ActionResult Index()
        {
            LoadViewBag();
            return View();
        }
        #endregion

        #region Add
        /// <summary>
        /// Ajoute une assignation de croisière
        /// </summary>
        /// <returns>Retourne la vue</returns>
        [HttpGet]
        public ActionResult _Create()
        {
            LoadViewBag();
            return PartialView(new CreateCruiseAssignmentViewModel());
        }

        /// <summary>
        /// Création d'une assignation d'une croisière
        /// </summary>
        /// <param name="model">Instance du nouveau droit d'accès</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create([Bind(Include = "Ship, Cruises, Deadline")] CreateCruiseAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _cruiseAssignmentClass._Create(model);
                    return Json(new { result = true, url = "/CruiseAssignment" });
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
            return PartialView(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification d'assignation des croisières
        /// </summary>
        /// <param name="id">Identifiant de l'assignation</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LoadViewBag();
            EditCruiseAssignmentViewModel model = _cruiseAssignmentClass.GetCruiseAssignment(id.Value);
            return PartialView(model);
        }

        /// <summary>
        /// Modification de l'assignation des croisières
        /// </summary>
        /// <param name="model">Instance de l'assignation des croisières</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(EditCruiseAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _cruiseAssignmentClass.Edit(model);
                    return Json(new { result = true, url = "/CruiseAssignment" });
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
            return PartialView(model);
        }
        #endregion

        #region Delete
        /// <returns></returns>
        /// <summary>
        /// Supprime une assignation des croisières
        /// </summary>
        /// <param name="id">Identifiant de l'assignation des croisières</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _cruiseAssignmentClass.Delete(id.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while delete this cruise assignment";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
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
            ViewBag.ShipList = _cruiseClass.GetLovList(Data.Constants.LOV_SHIP);
        }
        #endregion

        #endregion
    }
}