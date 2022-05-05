namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;

    [Authorize(Roles = "IT Administrator, Booking Administrator")]
    public class LanguageController : BaseController
    {
        #region Properties & Constructors

        private readonly LanguageClass _languageClass;

        public LanguageController()
        {
            _languageClass = new LanguageClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des langages associés au questionnaires
        /// </summary>
        /// <param name="idSurvey">Identifiant du questionnaire</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int? idSurvey)
        {
            if (idSurvey.HasValue)
            {
                ViewBag.IdSurvey = idSurvey;
            }
            return PartialView();
        }
        #endregion

        #region Create
        /// <summary>
        /// Formulaire de création d'un langage
        /// </summary>
        /// <param name="idSurvey">Identifiant du questionnaire</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Create(int? idSurvey)
        {
            CreateLanguageViewModel model = new CreateLanguageViewModel();
            if(idSurvey.HasValue)
            {
                model.IdSurvey = idSurvey.Value;
            }
            
            LoadViewBag();
            return PartialView(model);
        }

        /// <summary>
        ///  Création d'un langage
        /// </summary>
        /// <param name="model">Instance du nouveau langage</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(CreateLanguageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _languageClass.Create(model);
                    return Json(new { result = true, url = string.Format("/Survey/Edit/{0}", model.IdSurvey.ToString()) });
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
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EditLanguageViewModel model = _languageClass.GetLanguage(id.Value);
            LoadViewBag();
            ViewBag.LanguageIsInUse = _languageClass.IsInUse(id.Value);
            return PartialView(model);
        }

        /// <summary>
        ///  Modification d'un langage
        /// </summary>
        /// <param name="model">Instance du langage modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(EditLanguageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _languageClass.Edit(model);
                    return Json(new { result = true, url = string.Format("/Survey/Edit/{0}", model.IdSurvey.ToString()) });
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
        /// Suppression d'un langage
        /// </summary>
        /// <param name="id">Identifiant du langage</param>
        /// <returns>Redirection vers l'edition du survey</returns>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            int? IdSurvey = null;
            if (id.HasValue)
            {
                try
                {
                    if (!_languageClass.IsInUse(id.Value))
                    {
                        IdSurvey = _languageClass.Delete(id.Value);
                        if (!IdSurvey.HasValue)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        IdSurvey = (_languageClass.GetLanguage(id.Value)).IdSurvey;
                        TempData["ErrorMessage"] = "Language in use. Can not be deleted.";
                        LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.Language, LogManager.LogAction.Delete, User.Identity.Name ,"Delete language in use. Id : " + id.Value.ToString());
                    }
                }
                catch (Exception e)
                {     
                    TempData["ErrorMessage"] = "An error occurred while deleting this language";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Edit", "Survey", new { id = IdSurvey });
        }
        #endregion

        #region FileUpload
        /// <summary>
        /// Telechargement d'un fichier vers le serveur
        /// </summary>
        /// <param name="name">Nom du type de document</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileUpload(string name)
        {
            bool result = false;
            HttpPostedFileBase file = Request.Files[0];

            if (file != null)
            {
                string tmpFilename = User.Identity.Name + "_" + name + file.FileName;
                FileManager.FileSave(AppSettings.FolderTemp, tmpFilename, file);
                result = true;
            }

            return Json(result);
        }
        #endregion

        #region FileDownload
        /// <summary>
        /// Telechargement d'un fichier depuis le serveur
        /// </summary>
        /// <param name="idLanguage">Identifiant du langage</param>
        /// <param name="filetype">Type de fichier</param>
        /// <returns>Fichier a retourner</returns>
        [HttpGet]
        public ActionResult FileDownload(int? idLanguage, LanguageClass.FileLanguageType fileType)
        {
            if(!idLanguage.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string path = null;
            string filename = _languageClass.GetFileName(idLanguage.Value, fileType);
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            switch (fileType)
            {
                case LanguageClass.FileLanguageType.IndividualSurveyFileName:
                    path = AppSettings.FolderSurveyIndividual;
                    break;
                case LanguageClass.FileLanguageType.GroupSurveyFileName:
                    path = AppSettings.FolderSurveyGroup;
                    break;
                case LanguageClass.FileLanguageType.IndividualSurveyMail:
                    path = AppSettings.FolderMailIndividual;
                    break;      

                case LanguageClass.FileLanguageType.GroupSurveyMail:
                    path = AppSettings.FolderMailGroup;
                    break;
                case LanguageClass.FileLanguageType.IndividualAutomaticResponse:
                    path = AppSettings.FolderMailIndividualAutomaticResponse;
                    break;
                case LanguageClass.FileLanguageType.GroupAutomaticResponse:
                    path = AppSettings.FolderMailGroupAutomaticResponse;
                    break;

            }

            string filePath = FileManager.FileGetPath(path, (idLanguage + filename));
            if (string.IsNullOrEmpty(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = HttpContext.Server.UrlPathEncode(filename),
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());

            return (File(fileData, contentType) as FileResult);
        }
        #endregion

        #region FileDelete
        /// <summary>
        /// Suppression d'un document du langage
        /// </summary>
        /// <param name="idLanguage">Identifiant du langage</param>
        /// <param name="fileType">Type de fichie a supprimer</param>
        /// <returns>Redirection vers l'edition du survey</returns>
        [HttpGet]
        public ActionResult FileDelete(int? idLanguage, LanguageClass.FileLanguageType fileType)
        {
            int? IdSurvey = null;
            if (idLanguage.HasValue)
            {
                try
                {
                    IdSurvey = _languageClass.FileDelete(idLanguage.Value, fileType);
                    if (!IdSurvey.HasValue)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting this document";
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return RedirectToAction("Edit", "Survey", new { id = IdSurvey });
        }
        #endregion

        #region TemporaryFilesDelete
        /// <summary>
        /// Suppression des document temporaire
        /// </summary>
        /// <param name="suppressionType">Type de suppression</param>
        [HttpGet]
        public bool TemporaryFilesDelete(int suppressionType)
        {
            switch(suppressionType)
            {
                case 1: // Suppression par utilisateur
                    string userName = User.Identity.Name;
                    FileManager.FileManyDelete(AppSettings.FolderTemp, (userName + "_"));
                    break;
            }
            return true;
        }
        #endregion

        #region CheckDefaultValue
        /// <summary>
        /// Remote Validation Default
        /// </summary>
        /// <param name="defaultLanguage">Defaut à valider</param>
        /// <param name="idSurvey">Id du questionnaire courrant</param>
        /// <param name="id">Id du langage courrant</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckDefaultValue(bool defaultLanguage, int idSurvey, int id)
        {
            bool result = _languageClass.IsValidDefault(defaultLanguage, idSurvey, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private

        /// <summary>
        /// Chargement des liste du modéle commune entre _Create et _Edit
        /// </summary>
        private void LoadViewBag()
        {
            ViewBag.LangueList = _languageClass.GetLovList(Data.Constants.LOV_LANGUAGE);
        }

        #endregion
    }
}