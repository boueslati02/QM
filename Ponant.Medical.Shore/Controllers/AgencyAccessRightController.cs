using Ponant.Medical.Shore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ponant.Medical.Shore.Controllers
{
    [Authorize(Roles = "IT Administrator, Group")]
    public class AgencyAccessRightController : BaseController
    {
        #region Properties & Constructors
        private readonly AgencyAccessRightClass _agencyAccessRightClass;
        private readonly SharedPassengerClass _sharedPassengerClass;

        public AgencyAccessRightController()
        {
            _agencyAccessRightClass = new AgencyAccessRightClass(_shoreEntities);
            _sharedPassengerClass = new SharedPassengerClass(_shoreEntities);
        }
        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des droits d'accès et les filtres
        /// </summary>
        /// <returns>La liste des droits d'accès</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Create
        /// <summary>
        /// Formulaire de création d'un droit d'accès
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Create()
        {
            ViewBag.Cruises = _shoreEntities.Cruise.ToList();
            return PartialView(new CreateAgencyAccessRightViewModel());
        }

        /// <summary>
        /// Création du droit d'accès
        /// </summary>
        /// <param name="model">Instance du nouveau droit d'accès</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create([Bind(Include = "IdAgency, AgencyName, CruiseCode, GroupName, BookingNumber")] CreateAgencyAccessRightViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _agencyAccessRightClass._Create(model);

                    IEnumerable<Tuple<int, int>> ids =_agencyAccessRightClass.GetPassengers(model.BookingNumber.Value);

                    foreach (Tuple<int, int> id in ids)
                    {
                        _sharedPassengerClass.ChangeBookingPassengerState(id.Item1, id.Item2, false);
                    }

                    return Json(new { result = true, url = "/AgencyAccessRight" });
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

        #region GetAgencyList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetAgencyList(string name)
        {
            List<SelectListItem> agencies = (from agence in _shoreEntities.Agency.AsEnumerable()
                                             where !agence.Id.Equals(0) &&
                                             (agence.Name.ToUpper().Contains(name.ToUpper()) ||
                                             agence.Number.HasValue && agence.Number.ToString().Contains(name))
                                             orderby agence.Name ascending
                                             select new SelectListItem()
                                             {
                                                 Text = agence.Number.HasValue ? $"{agence.Number.Value} - {agence.Name}" : agence.Name,
                                                 Value = agence.Id.ToString()
                                             }).ToList();

            return Json(agencies, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification d'un droit d'accès
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Cruises = _shoreEntities.Cruise.ToList();
            EditAgencyAccessRightViewModel model = _agencyAccessRightClass.GetAgencyAccessRight(id.Value);
            return PartialView(model);
        }

        /// <summary>
        /// Modification d'un droit d'accès
        /// </summary>
        /// <param name="model">Instance du droit d'accès modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(EditAgencyAccessRightViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _agencyAccessRightClass.Edit(model);
                    return Json(new { result = true, url = "/AgencyAccessRight" });
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
        /// Supprime un droit d'accès
        /// </summary>
        /// <param name="id">Identifiant du droit d'accès</param>
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
                _agencyAccessRightClass.Delete(id.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while delete this agency";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}