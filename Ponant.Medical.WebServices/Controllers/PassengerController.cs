using Ponant.Medical.Common;
using Ponant.Medical.Common.Interfaces;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Ponant.Medical.WebServices.Controllers
{
    /// <summary>
    /// Gestion des ressources de passagers
    /// </summary>
    [Authorize]
    public class PassengerController : ApiController
    {
        #region Properties & Constructors

        private readonly IShoreEntities _shoreEntities;

        private readonly IFileHelper _fileHelper;

        private readonly IArchiveHelper _archiveHelper;

        /// <summary>
        /// Document Controller
        /// </summary>
        /// <param name="shoreEntities">Shore Entities</param>
        /// <param name="fileHelper">File Helper</param>
        /// <param name="archiveHelper">Archive Helper</param>
        public PassengerController(
            IShoreEntities shoreEntities,
            IFileHelper fileHelper,
            IArchiveHelper archiveHelper)
        {
            _shoreEntities = shoreEntities;
            _fileHelper = fileHelper;
            _archiveHelper = archiveHelper;
        }
        #endregion

        #region ChangeStatusPassenger
        /// <summary>
        /// Modification du statut de telechargement du passager lors du téléchargement avant la croisière
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <returns>Ok si le statut a bient été modifié</returns>
        [HttpPut]
        [Route("api/passenger/changestatuspassenger")]
        public IHttpActionResult ChangeStatusPassenger(int id)
        {
            try
            {
                Passenger passenger = _shoreEntities.Passenger.Single(p => p.Id.Equals(id));
                passenger.IsDownloaded = true;

                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, string.Format("Statut {0} pour le passager {1} {2}", true, passenger.LastName, passenger.FirstName));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Ok();
        }
        #endregion

        #region EditAdvice
        /// <summary>
        /// Met à jour un avis pour un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="advice">Données de l'avis du médecin</param>
        /// <returns>Ok si l'avis a bien été mis à jour permettant de valider l'acquittement à bord</returns>
        [HttpPut]
        [Route("api/passenger/editadvice")]
        public IHttpActionResult EditAdvice(int id, AdviceBoard advice)
        {
            Passenger passenger = null;

            if (!advice.IdPassenger.Equals(id))
            {
                return BadRequest("The two identifiers do not match.");
            }

            try
            {
                try
                {
                    passenger = _shoreEntities.Passenger.Single(p => p.Id.Equals(advice.IdPassenger));
                }
                catch(InvalidOperationException)
                {
                    return NotFound();
                }

                // Mise à jour du passager
                passenger.IdAdvice = advice.IdAdvice;
                passenger.IsExtract = false;
                passenger.Review = advice.Comments;
                passenger.Doctor = UserHelper.GetUserId(HttpContext.Current.User.Identity.Name);
                passenger.Editor = HttpContext.Current.User.Identity.Name;
                passenger.TreatmentDate = passenger.ModificationDate = DateTime.Now;

                // Mise à jour du statut
                passenger.IdStatus = (advice.IdAdvice == Constants.ADVICE_WAITING_FOR_CLARIFICATION)
                    ? Constants.SHORE_STATUS_QM_INCOMPLETE
                    : Constants.SHORE_STATUS_QM_CLOSED;

                // Mise à jour des informations complémentaires
                if (advice.Informations != null)
                {
                    passenger.Information = advice.Informations.Select(i => new Information
                    {
                        IdPassenger = passenger.Id,
                        IdInformation = i,
                        Creator = HttpContext.Current.User.Identity.Name,
                        CreationDate = DateTime.Now,
                        Editor = HttpContext.Current.User.Identity.Name,
                        ModificationDate = DateTime.Now
                    }).ToList();
                }

                _shoreEntities.SaveChanges();

                Lov lovAdvice = _shoreEntities.Lov.Single(l => l.Id.Equals(passenger.IdAdvice));
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Advice, HttpContext.Current.User.Identity.Name, string.Format("Mise à jour de l'avis {0} du passager {1} {2}", lovAdvice.Name, passenger.LastName, passenger.FirstName));
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Passenger, LogManager.LogAction.Advice, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Ok();
        }
        #endregion

        #region ExtractPassenger
        /// <summary>
        /// Blocage de la croisière et du passager lors du téléchargement des données sur le navire
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <returns>Ok si l'acquittement a bien été intégré</returns>
        [HttpPut]
        [Route("api/passenger/extractpassenger")]
        public IHttpActionResult ExtractPassenger(int id, int idCruise)
        {
            Passenger passenger = null;

            try
            {
                passenger = _shoreEntities.Passenger.Single(p => p.Id.Equals(id));
                passenger.IsExtract = true;
                passenger.IdStatus = Constants.SHORE_STATUS_QM_IN_PROGRESS;
                passenger.Doctor = HttpContext.Current.User.Identity.Name;
            }
            catch(InvalidOperationException)
            {
                return NotFound();
            }

            try
            {
                foreach (BookingCruisePassenger bookingCruisePassenger in passenger.BookingCruisePassenger
                    .Where(bcp => bcp.IdCruise.Equals(idCruise)))
                {
                    bookingCruisePassenger.Cruise.IsExtract = true;
                    bookingCruisePassenger.Cruise.Extract = bookingCruisePassenger.Editor = passenger.Editor = HttpContext.Current.User.Identity.Name;
                    bookingCruisePassenger.ModificationDate = passenger.ModificationDate = DateTime.Now;

                    foreach (Assignment assignment in _shoreEntities.Assignment
                        .Where(a => a.Cruises.Contains(bookingCruisePassenger.Cruise.Code)))
                    {
                        assignment.Cruises = assignment.Cruises.Replace(bookingCruisePassenger.Cruise.Code, "");
                    }
                }

                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Lock, HttpContext.Current.User.Identity.Name, string.Format("Extraction du passager {0} {1}", passenger.LastName, passenger.FirstName));
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Passenger, LogManager.LogAction.Lock, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }
            return Ok();
        }
        #endregion

        #region GetPassengersByCruise
        /// <summary>
        /// Retourne la liste des identifiants de passagers d'une croisière
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <param name="status">Identifiant du statut du QM</param>
        /// <param name="forCurrentCruises">Indique que la récupération des passagers se fait pour les croisières à venir</param>
        /// <returns>Liste des identifiants de passagers de la croisière</returns>
        [HttpGet]
        [Route("api/passenger/getpassengersbycruise")]
        public IHttpActionResult GetPassengersByCruise(int id, int? status, bool forCurrentCruises = false)
        {
            List<PassengerCruise> passengersCruises;

            try
            {
                passengersCruises = _shoreEntities.vPassengerBoard
                    .Where(p => (forCurrentCruises || !p.IsExtract || p.Doctor.Equals(HttpContext.Current.User.Identity.Name))
                    && p.IdCruise.Equals(id) && p.IdStatus.Equals(status.HasValue ? status.Value : p.IdStatus))
                    .Select(p => new PassengerCruise
                    {
                        IdPassenger = p.Id,
                        IdCruise = p.IdCruise
                    }).ToList();
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Passenger, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Json(passengersCruises);
        }
        #endregion

        #region GetPassenger
        /// <summary>
        /// Retourne les informations sur un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <returns>Informations sur le passager</returns>
        [HttpGet]
        public IHttpActionResult GetPassenger(int id, int idCruise)
        {
            vPassengerBoard passenger = new vPassengerBoard();

            try
            {
                // Récupération du passager
                passenger = _shoreEntities.vPassengerBoard
                    .Single(p => p.Id.Equals(id) && p.IdCruise.Equals(idCruise));

                // Récupération des informations complémentaires
                passenger.Informations = _shoreEntities.Information
                    .Where(i => i.IdPassenger == passenger.Id)
                    .Select(i => i.IdInformation).ToList();

                // Récupération des documents
                List<Document> documents = _shoreEntities.Document
                    .Where(d => d.IdPassenger == passenger.Id)
                    .ToList();

                if (documents != null && documents.Count > 0)
                {
                    passenger.DocumentsInfos = new Dictionary<int, string>();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Création d'une archive pour tous les documents
                        using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        {
                            foreach (Document document in documents)
                            {
                                string path = Path.Combine(AppSettings.FolderPassenger, document.ReceiptDate.ToString("yyyy-MM"), passenger.Id.ToString(), document.FileName);
                                string entryName = Path.GetFileNameWithoutExtension(document.FileName) + Path.GetExtension(document.Name);

                                if (_fileHelper.FileExists(path))
                                {
                                    // Crée une entrée dans l'archive
                                    ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                                    using (Stream stream = entry.Open())
                                    {
                                        // Décompresse le document stocké compressé
                                        byte[] bytes = _archiveHelper.UnZip(path);

                                        if (bytes != null)
                                        {
                                            stream.Write(bytes, 0, bytes.Length);
                                        }
                                    }
                                }

                                passenger.DocumentsInfos.Add(document.Id, entryName);
                            }
                        }

                        passenger.Documents = ms.ToArray();
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Passenger, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, exception);
                return InternalServerError(exception);
            }

            return Json(passenger);
        }
        #endregion
    }
}
