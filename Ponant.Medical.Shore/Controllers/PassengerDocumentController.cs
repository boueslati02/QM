namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;

    [Authorize(Roles = "Medical, Medical Administrator, Doctor")]
    public class PassengerDocumentController : BaseController
    {
        #region Properties & Constructors

        private readonly PassengerDocumentClass _passengerDocumentClass;

        public PassengerDocumentController()
        {
            _passengerDocumentClass = new PassengerDocumentClass(_shoreEntities);
        }
        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des passagers
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region _Create
        /// <summary>
        /// Formulaire d'ajout d'un nouveau passager
        /// </summary>
        /// <param name="cruiseCode">Code de la croisière</param>
        /// <param name="bookingCode">Code du booking</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Create(string cruiseCode, string bookingCode)
        {
            CreatePassengerDocumentViewModel model = new CreatePassengerDocumentViewModel
            {
                CruiseCode = !string.IsNullOrEmpty(cruiseCode) ? cruiseCode : null,
                BookingCode = !string.IsNullOrEmpty(bookingCode) ? bookingCode : null
            };
            return PartialView(model);
        }

        /// <summary>
        ///  Ajout d'un nouveau passager avec document associé
        /// </summary>
        /// <param name="model">Instance du nouveau passager</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(CreatePassengerDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _passengerDocumentClass.AddPassenger(model);
                    TempData["Message"] = "This passenger have been correctly create";
                    return Json(new { result = true, url = "/PassengerDocument" });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while create this passenger. " + ex.Message;
                    ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                }
            }
            return PartialView(model);
        }
        #endregion

        #region _AddDocument
        /// <summary>
        /// Formulaire d'ajout d'un nouveau ducument à un passager
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _AddDocument(int? idPassenger)
        {
            if (!idPassenger.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AddDocumentViewModel model = _passengerDocumentClass.GetPassengerForAddDocument(idPassenger.Value);        
            return PartialView(model);
        }

        /// <summary>
        ///  Ajout d'un nouveau document
        /// </summary>
        /// <param name="model">Instance du nouveau document</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _AddDocument(AddDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _passengerDocumentClass.AddDocument(model);
                    TempData["Message"] = "This documents have been correctly added";
                    return Json(new { result = true, url = "/PassengerDocument" });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while add this document. " + ex.Message;
                    ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                }
            }
            return PartialView(model);
        }
        #endregion

        #region Detach
        /// <summary>
        /// Detache le documents indiqué de son passager 
        /// <param name="idDocument">Identifiant des document à détaché</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detach(int? idDocument)
        {
            if (!idDocument.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _passengerDocumentClass.DetachDocument(idDocument.Value);
                TempData["Message"] = "This documents have been correctly detached";
                return Json(new { result = true, url= "/PassengerDocument" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while detached this document. " + ex.Message;
                ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
            }
            return RedirectToAction("Index");
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

        #region TemporaryFilesDelete
        /// <summary>
        /// Suppression des document temporaire
        /// </summary>
        /// <param name="suppressionType">Type de suppression</param>
        [HttpGet]
        public bool TemporaryFilesDelete(int suppressionType)
        {
            switch (suppressionType)
            {
                case 1: // Suppression par utilisateur
                    string userName = User.Identity.Name;
                    FileManager.FileManyDelete(AppSettings.FolderTemp, (userName + "_PassengerDocument"));
                    break;
            }
            return true;
        }
        #endregion

        #region CheckCruiseValue
        /// <summary>
        /// Remote Validation Cruise
        /// </summary>
        /// <param name="cruiseCode">Code de croisière à valider</param>
        /// <returns>Vrai si la croisière existe, faux sinon</returns>
        [HttpGet]
        public ActionResult CheckCruiseValue(string cruiseCode)
        {
            bool result = _passengerDocumentClass.IsValidCruiseCode(cruiseCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CheckBookingValue
        /// <summary>
        /// Remote Validation Booking
        /// </summary>
        /// <param name="bookingCode">Booking à valider</param>
        /// <param name="cruiseCode">Code de la croisère</param>
        /// <returns>Vrai si la reservation existe, faux sinon</returns>
        [HttpGet]
        public ActionResult CheckBookingValue(string bookingCode, string cruiseCode)
        {
            bool result = _passengerDocumentClass.IsValidBookingCode(bookingCode, cruiseCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}