namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [Authorize(Roles = "Medical, Medical Administrator, Doctor")]
    public class MedicalController : BaseController
    {
        #region Properties & Constructors

        private readonly MedicalClass _medicalClass;

        private readonly PassengerDocumentClass _passengerDocumentClass;

        public MedicalController()
        {
            _medicalClass = new MedicalClass(_shoreEntities, _applicationDbContext);
            _passengerDocumentClass = new PassengerDocumentClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des passagers
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisiére</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int? idCruise, int? tabsToDisplay, bool? emailError = null)
        {
            if (emailError.GetValueOrDefault(false)) // Affichage du message d'erreur lorsque l'email n'est pas envoyé
            {
                TempData["ErrorMessage"] = "Error sending information email";
                return RedirectToAction("Index", new { idCruise, tabsToDisplay });
            }

            MedicalViewModel model = new MedicalViewModel();
            if (idCruise.HasValue)
            {
                model = _medicalClass.GetCruiseData(idCruise.Value);
            }

            LoadViewBag(tabsToDisplay);
            return View(model);
        }
        #endregion

        #region ChangeState
        /// <summary>
        /// Change l'état d'un passager pour la croisière
        /// </summary>
        /// <param name="id">Identifiant du passagers</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="enable">Etat du passager</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeState(int? id, int? idCruise, int? tabsToDisplay, bool? enable)
        {
            if (!id.HasValue || !idCruise.HasValue || !enable.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _medicalClass.ChangeBookingPassengerState(id.Value, idCruise.Value, enable.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while update this passenger state";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
            }

            return RedirectToAction("Index", new { idCruise, tabsToDisplay });
        }
        #endregion

        #region _GetDocuments
        /// <summary>
        /// Fenetre de consulation des documents du passagers
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _GetDocuments(int? id, int? idCruise)
        {
            if (!id.HasValue || !idCruise.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GetDocumentsMedicalViewModel model = _medicalClass.GetPassengerForDocument(id.Value);
            model.IdCruise = idCruise.Value;
            ViewBag.SeenText = (from s in _medicalClass.GetLovList(Constants.LOV_DOCUMENT_STATUS) where s.Value.Equals(Constants.DOCUMENT_STATUS_SEEN.ToString()) select s.Text).FirstOrDefault();

            return PartialView(model);
        }
        #endregion

        #region _EditAdvice
        /// <summary>
        /// Fenetre de modification de l'avis médical
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="cruiseId">Identifiant de la croisière</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public ActionResult _EditAdvice(int? id, int? idCruise)
        {
            if (!id.HasValue || !idCruise.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EditAdviceViewModel model = _medicalClass.GetPassengerForAdvice(id.Value);
            model.IdCruise = idCruise.Value;

            LoadAdviceViewBag();
            return PartialView(model);
        }

        /// <summary>
        /// Modification de l'avis médical d'un passager
        /// </summary>
        /// <param name="model">Instance du passager modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> _EditAdvice(EditAdviceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool emailError = !await _medicalClass.EditAdvice(model);
                    return Json(new { result = true, url = string.Format("/Medical?idCruise={0}&emailError={1}", model.IdCruise.ToString(), emailError) });
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
            LoadAdviceViewBag();
            return PartialView(model);
        }
        #endregion

        #region FileDownload
        /// <summary>
        /// Telechargement d'un fichier depuis le serveur
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns>Fichier a retourner</returns>
        [HttpGet]
        public ActionResult FileDownload(int? idDocument)
        {
            if (!idDocument.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Document document = _passengerDocumentClass.GetDocument(idDocument.Value);
            if (document.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string filePath = FileManager.FileGetPath(_passengerDocumentClass.GetDocumentPath(document), document.FileName);
            if (string.IsNullOrEmpty(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            byte[] fileData = null;
            string contentType = MimeMapping.GetMimeMapping(document.Name);

            if (System.IO.Path.GetExtension(filePath).ToLower() == ".zip")
            {
                Archive zip = new Archive();
                fileData = zip.UnZip(filePath);
            }
            else
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
            }

            if (fileData == null || contentType == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = HttpContext.Server.UrlPathEncode(document.Name),
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());

            return (File(fileData, contentType) as FileResult);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Supprime un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int? id, int? idCruise)
        {
            if (!id.HasValue || !idCruise.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _medicalClass.Delete(id.Value, idCruise.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while delete this passenger";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
            }

            return RedirectToAction("Index", new { idCruise });
        }
        #endregion

        #region CheckRestriction
        /// <summary>
        /// Remote Validation Order
        /// </summary>
        /// <param name="order">Ordre à valider</param>
        /// <param name="id">Id du critére courrant</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckRestriction(List<int> restrictionsTypes, int idAdvice, string restrictionsString)
        {
            bool result = true;
          
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Méthode privées

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune
        /// </summary>
        private void LoadViewBag(int? tabsToDisplay = null)
        {
            ViewBag.AdviceList = _medicalClass.GetLovList(Constants.LOV_ADVICE);
            ViewBag.DoctorList = _medicalClass.GetDoctorList();
            ViewBag.TabsToDisplay = tabsToDisplay ?? 0;
        }
        #endregion

        #region LoadAdviceViewBag
        /// <summary>
        /// Chargement des liste du modéle pour la fenetre d'édition de l'avis médical
        /// </summary>
        private void LoadAdviceViewBag()
        {
            ViewBag.AdviceList = _medicalClass.GetLovList(Constants.LOV_ADVICE);
            ViewBag.DocumentsTypesList = _medicalClass.GetLovList(Constants.LOV_ADDITIONAL_DOCUMENTS);
            ViewBag.UnfavorableOpinionList = _medicalClass.GetLovList(Constants.LOV_UNFAVORABLE_ADVICE);
            ViewBag.RestrictionsTypesList = _medicalClass.GetLovList(Constants.LOV_RESTRICTION_ADVICE);
            ViewBag.RestrictionsPeopleList = _medicalClass.GetLovList(Constants.LOV_RESTRICTION_PERSON);
        }
        #endregion

        #endregion
    }
}