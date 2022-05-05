namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Data.Entity.Validation;
    using System.Net;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator")]
    public class SurveyController : BaseController
    {
        #region Properties & Constructors

        private readonly SurveyClass _surveyClass;

        public SurveyController()
        {
            _surveyClass = new SurveyClass(_shoreEntities);
        }
        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des questionnaires
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
        /// Formulaire de création d'un questionnaire
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            CreateSurveyViewModel model = new CreateSurveyViewModel();
            return View(model);
        }

        /// <summary>
        ///  Création d'un questionnaire
        /// </summary>
        /// <param name="model">Instance du nouveau questionnairer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateSurveyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? surveyId = _surveyClass.Create(model);
                    if (surveyId.HasValue)
                    {
                        return RedirectToAction("Edit", new { id = surveyId.Value });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while adding this survey";
                        return RedirectToAction("Index");
                    }
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
            return View(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Formulaire de modification d'un questionnaire
        /// <param name="id">Identifiant du questionnaire</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.SurveyIsInUse = _surveyClass.IsInUse(id.Value);
            EditSurveyViewModel model = _surveyClass.GetSurvey(id.Value);
            return View(model);
        }

        /// <summary>
        ///  Modification d'un questionnaire
        /// </summary>
        /// <param name="model">Instance du questionnaire modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditSurveyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _surveyClass.Edit(model);
                    return RedirectToAction("Index");
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
        /// <summary>
        /// Suppression d'un questionnaire
        /// </summary>
        /// <param name="id">Identifiant du questionnaire</param>
        /// <returns>Redirection vers la liste des questionnaires</returns>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    if (!_surveyClass.IsInUse(id.Value))
                    {
                        _surveyClass.Delete(id.Value);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Survey in use. Can not be deleted.";
                        LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.Survey, LogManager.LogAction.Delete, User.Identity.Name, "Delete survey in use. Id : " + id.Value.ToString());
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting this survey";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}