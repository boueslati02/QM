namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;

    [Authorize(Roles = "Medical, Medical Administrator, Doctor")]
    public class AvailableDocumentController : BaseController
    {
        #region Properties & Constructors

        private readonly AvailableDocumentClass _availableDocumentClass;

        private readonly PassengerDocumentClass _passengerDocumentClass;

        public AvailableDocumentController()
        {
            _availableDocumentClass = new AvailableDocumentClass(_shoreEntities);
            _passengerDocumentClass = new PassengerDocumentClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des document disponible
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LoadViewBag();
            return View();
        }
        #endregion

        #region _GetMessage
        /// <summary>
        /// Fenetre de consulation du message de l'email
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _GetMessage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GetMessageViewModel model = _availableDocumentClass.GetMessageDocument(id.Value);
            return PartialView(model);
        }
        #endregion

        #region _Link
        /// <summary>
        /// Retourne la fenetre de liaison des document a un passagers 
        /// </summary> (Passage de la liste via une requête Post pour corriger les problèmes de transmissions de la liste d'identifiant)
        /// <param name="idsDocuments">Liste des identifiant des document à lié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _LinkGet(List<int> idsDocuments, string controllerName = "AvailableDocument")
        {
            if (idsDocuments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LinkViewModel model = _availableDocumentClass.GetLinkModel(idsDocuments);
            TempData["ControllerName"] = controllerName;
            return PartialView("_Link", model);
        }

        /// <summary>
        /// Liaison des document a un passagers
        /// </summary>
        /// <param name="model">Instance des documents à lier</param>
        /// <param name="controllerName">Nom du coontrolleur de retour</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _LinkPost(LinkViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.IdPassenger.HasValue)
                {
                    return PartialView("_Link", model);
                }

                string controllerName = string.IsNullOrEmpty(TempData["ControllerName"].ToString()) ? "AvailableDocument" : TempData["ControllerName"].ToString();
                try
                {
                    int linkDocument = 0;
                    int errorLink = 0;
                    foreach (int id in model.IdsDocument)
                    {
                        try
                        {
                            _availableDocumentClass.Link(id, model.IdPassenger.Value);
                            linkDocument++;
                        }
                        catch
                        {
                            errorLink++;
                        }
                    }

                    if (errorLink > 0)
                    {
                        throw new Exception(errorLink.ToString() + " documents have not been linked");
                    }
                    TempData["Message"] = linkDocument.ToString() + " documents have been correctly linked";
                    return Json(new { result = true, url = "/" + controllerName });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while link these documents. " + ex.Message;
                    ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                    return Json(new { result = true, url = "/" + controllerName });
                }
            }
            return PartialView("_Link", model);
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

        #region Detach
        /// <summary>
        /// Detache les documents indiqué de leur passagers
        /// </summary>
        /// <param name="idsDocuments">Liste des identifiant des document à détaché</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detach(List<int> idsDocuments)
        {
            if (idsDocuments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                int detachDocument = 0;
                int errorDetach = 0;
                foreach (int id in idsDocuments)
                {
                    try
                    {
                        _availableDocumentClass.DetachDocument(id);
                        detachDocument++;
                    }
                    catch (Exception)
                    {
                        errorDetach++;
                    }
                }

                if (errorDetach > 0)
                {
                    throw new Exception(errorDetach.ToString() + " documents have not been detached");
                }
                TempData["Message"] = detachDocument.ToString() + " documents have been correctly detached";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while detached these documents. " + ex.Message;
                ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
            }

            return Json(new { result = true, url = "/AvailableDocument" });
        }
        #endregion

        #region Delete
        /// <summary>
        /// Supprime les documents indiqué
        /// </summary>
        /// <param name="idsDocuments">Liste des identifiant des document à supprimer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete (List<int> idsDocuments)
        {
            if (idsDocuments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                int deletedDocument = 0;
                int errorDelete = 0;
                foreach (int id in idsDocuments)
                {
                    try
                    {
                        _availableDocumentClass.Delete(id);
                        deletedDocument++;
                    }
                    catch (Exception)
                    {
                        errorDelete++;
                    }
                }

                if(errorDelete > 0)
                {
                    throw new Exception(errorDelete.ToString() + " documents have not been deleted");
                }
                TempData["Message"] = deletedDocument.ToString() + " documents have been correctly deleted";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while delete these documents. " + ex.Message;
                ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return Json(new { result = true, url = "/AvailableDocument" });
        }
        #endregion

        #region Fonctions privées

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune
        /// </summary>
        private void LoadViewBag()
        {
            List<SelectListItem> documentAvailableList = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Yes", Value = "1" },
                new SelectListItem() { Text = "No", Value = "0", Selected = true }
            };
            ViewBag.DocumentAvailableList = documentAvailableList;
        }
        #endregion

        #endregion
    }
}