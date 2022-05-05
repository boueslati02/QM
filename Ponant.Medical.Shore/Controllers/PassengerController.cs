namespace Ponant.Medical.Shore.Controllers
{
    using Ponant.Medical.Common.Ssrs;
    using Ponant.Medical.Data;
    using Ponant.Medical.Shore.Helpers;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Booking, Booking Administrator,IT Administrator, Group, Agency, Agency Administrator")]
    public class PassengerController : BaseController
    {
        #region Properties & Constructors

        private readonly PassengerClass _passengerClass;

        public PassengerController()
        {
            _passengerClass = new PassengerClass(_shoreEntities);
        }

        #endregion

        #region Index
        /// <summary>
        /// Affiche la liste des passagers
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisiére</param>
        /// <param name="bookingGroupToDisplay">Identifiant du booking de groupe à afficher</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int? idCruise, int? bookingGroupToDisplay)
        {
            PassengerViewModel model = new PassengerViewModel();
            if (idCruise.HasValue)
            {
                model = _passengerClass.GetCruiseData(idCruise.Value);

                if  (!bookingGroupToDisplay.HasValue && model.ListOfGroups.Any() &&
                    (System.Web.HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) ||
                     System.Web.HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR)))
                {
                    bookingGroupToDisplay = model.ListOfGroups[0].Id;
                }

            }
            LoadViewBag(bookingGroupToDisplay);
            return View(model);
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
        public ActionResult _Create(int cruiseId, int bookingNumber)
        {
            CreateAgencyPassengerViewModel model = new CreateAgencyPassengerViewModel
            {
                CruiseId = cruiseId,
                BookingNumber = bookingNumber
            };

            LoadViewBag();

            return PartialView(model);
        }

        /// <summary>
        ///  Ajout d'un nouveau passager
        /// </summary>
        /// <param name="model">Instance du nouveau passager</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create(CreateAgencyPassengerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? idPassenger = _passengerClass.AddPassenger(model);
                    List<int> idRelaunchPassengers = new List<int>();
                    idRelaunchPassengers.Add(idPassenger.Value);
                    await CheckAndSendIndividualRelaunchAsync(idRelaunchPassengers, model.CruiseId, true);

                    TempData["Message"] = "This passenger have been correctly create";
                    return Json(new { result = true, url = "/Passenger?idCruise=" + model.CruiseId });
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

        #region Edit
        /// <summary>
        /// Formulaire de modification d'un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="cruiseId">Identifiant de la croisiére</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Edit(int? id, int? idCruise, int? idBooking)
        {
            if (id == null || idCruise == null || idBooking == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EditPassengerViewModel model = _passengerClass.GetPassenger(id.Value);
            model.CruiseId = idCruise.Value;
            model.BookingId = idBooking.Value;

            return PartialView(model);
        }

        /// <summary>
        /// Modification d'un passager individuel
        /// </summary>
        /// <param name="model">Instance du passager modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Edit(EditPassengerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _passengerClass.Edit(model);
                    bool isPassengerGroup = _passengerClass.IsGroupPassenger(model.BookingId);
                    TempData["Message"] = "This passenger have been correctly update";
                    if (isPassengerGroup)
                    {
                        return Json(new { result = true, url = string.Format("/Passenger/Index?idCruise={0}&bookingGroupToDisplay={1}", model.CruiseId, model.BookingId) });
                    }
                    return Json(new { result = true, url = string.Format("/Passenger/Index?idCruise={0}", model.CruiseId) });
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
                    TempData["ErrorMessage"] = "An error occurred while update this passenger. " + e.Message;
                    ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
                }
            }
            return PartialView(model);
        }
        #endregion

        #region Edit Agency
        /// <summary>
        /// Formulaire de modification d'un passager d'un agence
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="cruiseId">Identifiant de la croisiére</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditAgency(int? id, int? idCruise, int? idBooking)
        {
            if (id == null || idCruise == null || idBooking == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EditAgencyPassengerViewModel model = _passengerClass.GetAgencyPassenger(id.Value, idBooking.Value);
            model.CruiseId = idCruise.Value;
            model.BookingId = idBooking.Value;
            LoadViewBag();
            return PartialView(model);
        }

        /// <summary>
        /// Formulaire de modification d'un passager d'un agence
        /// </summary>
        /// <param name="EditAgencyPassengerViewModel">Edition du passager</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _EditAgency(EditAgencyPassengerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? idPassenger = _passengerClass.EditAgencyPassenger(model);

                    if (model.ReSendSurvey && idPassenger.HasValue)
                    {
                        List<int> idRelaunchPassengers = new List<int>();
                        idRelaunchPassengers.Add(idPassenger.Value);
                        await CheckAndSendIndividualRelaunchAsync(idRelaunchPassengers, model.CruiseId, true);
                    }
                    TempData["Message"] = "This passenger have been correctly update";
                    return Json(new { result = true, url = string.Format("/Passenger/Index?idCruise={0}&bookingGroupToDisplay={1}", model.CruiseId, model.BookingId) });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while update this passenger. " + ex.Message;
                    ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                }
            }
            return PartialView(model);
        }

        #endregion

        #region ConfirmRelaunch
        /// <summary>
        /// Affiche la modal de confirmation de changement de status de QM
        /// </summary>
        /// <returns>Modal</returns>
        [HttpGet]
        public ActionResult _ConfirmRelaunch()
        {
            return PartialView();
        }
        #endregion

        #region ChangeState
        /// <summary>
        /// Change l'état d'un passager pour la croisière
        /// </summary>
        /// <param name="id">Identifiant du passagers</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idBooking">Identifiant du booking de groupe à affiché</param>
        /// <param name="enable">Etat du passager</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeState(int? id, int? idCruise, int? idBooking, bool? enable)
        {
            if (!id.HasValue || !idCruise.HasValue || !enable.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _passengerClass.ChangeBookingPassengerState(id.Value, idCruise.Value, enable.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while update this passenger state";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
            }

            return RedirectToAction("Index", new { idCruise, bookingGroupToDisplay = idBooking });
        }
        #endregion

        #region Delete
        /// <returns></returns>
        /// <summary>
        /// Supprime un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idBooking">Identifiant du booking de groupe à afficher</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int? id, int? idCruise, int? idBooking)
        {
            if (!id.HasValue || !idCruise.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _passengerClass.Delete(id.Value, idCruise.Value);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "An error occurred while delete this passenger";
                ModelState.AddModelError("", string.Concat(e.Message, e.InnerException?.Message));
            }

            LoadViewBag(idBooking.Value);
            return RedirectToAction("Index", new { idCruise, bookingGroupToDisplay = idBooking });
        }
        #endregion

        #region IndividualRelaunch
        /// <summary>
        /// Envoi les relance des passagers individuel (appel via ajax jquery)
        /// </summary>
        /// <param name="idRelaunchPassengers">Liste des identifiants des passagers</param>
        /// <param name="idCruise">Identifiant de la croisiére</param>
        /// <returns>Json avec resultat et message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndividualRelaunch(List<int> idRelaunchPassengers, int? idCruise, bool statusNeedToBeUpdate = false)
        {
            if (idRelaunchPassengers == null || !idCruise.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool resultValue = false;
            string messageValue = null;
            PassengerClass.CounterRelaunchResult resultCounter = null;
            try
            {
                resultCounter = await _passengerClass.SendIndividualRelaunch(idRelaunchPassengers, idCruise.Value, statusNeedToBeUpdate);
                if (resultCounter != null && resultCounter.NbErrorMail > 0)
                {
                    throw new ArgumentException("Individual result counter error > 0");
                }

                messageValue = "Relaunch were sent correctly : " + resultCounter?.NbSendSurveyMail.ToString() + " relaunch for surveys, " + resultCounter?.NbSendAdditionalMail.ToString() + " relaunch for additionals documents.";
                if (statusNeedToBeUpdate)
                {
                    messageValue += " " + resultCounter?.NbUpdatePassengerStatus.ToString() + " passenger(s) have been updated from Qm received to Qm sent";
                }
                resultValue = true;
            }
            catch (Exception ex)
            {
                messageValue = "An error occurred while sending relaunch for checked passengers. ";
                messageValue += (resultCounter != null && resultCounter.NbErrorMail > 0) ? resultCounter.NbErrorMail.ToString() + " has not been sent." : ex.Message;
            }

            return Json(new { success = resultValue, message = messageValue });
        }
        #endregion

        #region ExportGroupPassenger
        /// <summary>
        /// Exporte les passagers au format Excel
        /// </summary>
        /// <param name="id">Identifiant du booking</param>
        /// <returns>Excel avec la liste des passagers</returns>
        [HttpGet]
        public FileResult ExportGroupPassenger(int id)
        {
            SsrsRender ssrsRender = new SsrsRender(AppSettings.SsrsWsUrl, AppSettings.SsrsUserName, AppSettings.SsrsPassword, AppSettings.SsrsDomain);
            byte[] file = ssrsRender.Render(AppSettings.ReportPathPassengersGroupExport, SsrsRender.RenderReportFormat.EXCEL, new Dictionary<string, string> { { "IdBooking", id.ToString() } });
            return File(file, "application/vnd.ms-excel", $"{id}-ExportPassengers.xls");
        }
        #endregion

        #region DownloadTemplate
        /// <summary>
        /// Télécharge le template d'import de passagers
        /// </summary>
        /// <param name="id">Identifiant du booking</param>
        /// <returns>CSV formaté</returns>
        [HttpGet]
        public FileResult DownloadTemplate(int id)
        {

            byte[] file = _passengerClass.GetTemplateFile(id);
            int BookingNumber = _passengerClass.GetBookingNumber(id);

            return File(file, "text/csv", $"{BookingNumber}-ImportPassengers.csv");
        }
        #endregion

        #region ImportPassenger
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ImportPassenger(int? id, int? idCruise)
        {
            if (id == null || idCruise == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                List<int> idPassengers = _passengerClass.ImportPassenger(Request.Files[0].InputStream, id.Value, idCruise.Value);
                if (idPassengers.Any())
                {
                    await CheckAndSendIndividualRelaunchAsync(idPassengers, idCruise, true);
                    TempData["Message"] = "Passengers have been correctly imported";
                }
                else
                {
                    TempData["ErrorMessage"] = "The booking number of a passenger does not match with the current group";
                }
                return Json(new { result = true, url = "/Passenger?idCruise=" + idCruise });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while importing passengers." + ex.Message;
                ModelState.AddModelError("", string.Concat(ex.Message, ex.InnerException?.Message));
                return Json(new { result = false, url = "/Passenger?idCruise=" + idCruise });
            }
        }
        #endregion

        #region IsAgencyPonant
        /// <summary>
        /// vérifie si le passager appartient à l'agence Ponant
        /// </summary>
        /// <param name="idAgency">Id de l'agence</param>
        /// <returns>bool</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IsAgencyPonant(int? idAgency)
        {
            if (!idAgency.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool isAgencyPonant = false;
            bool isAgency = false;

            if (idAgency == AppSettings.IdAgency)
            {
                isAgencyPonant = true;
            }

            if (System.Web.HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) ||
                System.Web.HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
            {
                isAgency = true;
            }

            return Json(new { IsAgencyPonant = isAgencyPonant, IsAgency = isAgency }, JsonRequestBehavior.AllowGet);
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
                    FileManager.FileManyDelete(AppSettings.FolderTemp, (userName + "_Passenger"));
                    break;
            }
            return true;
        }
        #endregion

        #region Méthode privées

        #region LoadViewBag
        /// <summary>
        /// Chargement des liste du modéle commune
        /// </summary>
        /// <param name="bookingGroupToDisplay">Id du groupe à affiché</param>
        private void LoadViewBag(int? bookingGroupToDisplay = null)
        {
            ViewBag.ListQmStatus = _passengerClass.GetLovList(Constants.LOV_SHORE_STATUS);
            ViewBag.ListOffice = _passengerClass.GetLovList(Constants.LOV_OFFICE);

            ViewBag.BookingGroupToDisplay = bookingGroupToDisplay ?? 0;

            ViewBag.Civilities = _passengerClass.GetLovList(Constants.LOV_CIVILITY);
        }
        #endregion

        #region CheckAndSendIndividualRelaunch
        public async Task CheckAndSendIndividualRelaunchAsync(List<int> idPassengers, int? CruiseId, bool statusNeedToBeUpdate = false)
        {
            if (idPassengers.Any() && CruiseId.HasValue)
            {
                PassengerClass.CounterRelaunchResult resultCounter = null;
                resultCounter = await _passengerClass.SendIndividualRelaunch(idPassengers, CruiseId.Value, true);
                if (resultCounter != null && resultCounter.NbErrorMail > 0)
                {
                    throw new ArgumentException("Individual result counter error > 0");
                }
            }
        }
        #endregion

        #endregion
    }
}