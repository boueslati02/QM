namespace Ponant.Medical.Shore.Models
{
    using MsgReader.Outlook;
    using Ponant.Medical.Common;
    using Ponant.Medical.Common.MailServer;
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Classe de méthode commune aux autres classe traitant les passagers
    /// </summary>
    public class SharedPassengerClass : SharedClass
    {
        #region Properties & Constructors

        public SharedPassengerClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region Customize Object

        #region CounterRelaunchResult
        /// <summary>
        /// Representation de l'object complexe CounterRelaunchResult
        /// </summary>
        public class CounterRelaunchResult
        {
            #region Properties

            public int NbTotalRelauchPassengers { get; set; }

            public int NbPassengersWithoutEmail { get; set; }

            public int NbSendSurveyMail { get; set; }

            public int NbSendAdditionalMail { get; set; }

            public int NbErrorMail { get; set; }

            public int NbUpdatePassengerStatus { get; set; }

            #endregion
        }
        #endregion

        #endregion

        #region ChangeBookingPassengerState
        /// <summary>
        /// Change l'état du passagers pour une croisière donnée
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="enable">Etat du passager</param>
        public void ChangeBookingPassengerState(int idPassenger, int idCruise, bool enable)
        {
            BookingCruisePassenger bookingPassenger = null;
            try
            {
                bookingPassenger = (from bcp in _shoreEntities.BookingCruisePassenger
                                    where bcp.IdPassenger.Equals(idPassenger) && bcp.IdCruise.Equals(idCruise)
                                    select bcp).FirstOrDefault();

                bookingPassenger.IsEnabled = enable;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name,
                    string.Concat("Edit isEnabled passenger Id : ", idPassenger.ToString(), " for cruise Id : ", idCruise.ToString(), " to : ", enable.ToString()),
                    bookingPassenger.Booking.Number);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name,
                    string.Concat("Edit isEnabled passenger Id : ", idPassenger.ToString(), " for cruise Id : ", idCruise.ToString(), " to : ", enable.ToString(), " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null), ")"),
                    (bookingPassenger != null ? (int?)bookingPassenger.Booking.Number : null));
                throw;
            }
        }
        #endregion

        #region SendRelaunchByPassenger
        /// <summary>
        /// Envoi des email de relance des passagers individuels
        /// </summary>
        /// <param name="passengers">Liste des passagers concernés</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="additionnalRecipients">Destinataires complémentaires</param>
        /// <returns>Objet de représentation des compteurs de résultat</returns>
        public async Task<CounterRelaunchResult> SendRelaunchByPassenger(List<Passenger> passengers, int idCruise, Dictionary<string, string> additionnalRecipients = null, bool withoutPassengerSave = false, bool statusNeedToBeUpdate = false)
        {
            // Initialisation des compteur de resultat
            CounterRelaunchResult resultCounter = new CounterRelaunchResult()
            {
                NbTotalRelauchPassengers = 0,
                NbPassengersWithoutEmail = 0,
                NbSendSurveyMail = 0,
                NbSendAdditionalMail = 0,
                NbErrorMail = 0,
                NbUpdatePassengerStatus = 0
            };

            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            try
            {
                CruiseCriteria cruiseCriteria = new CruiseCriteria(_shoreEntities);
                IEnumerable<BookingCruisePassenger> bookingCruisePassengers = passengers
                    .SelectMany(p => p.BookingCruisePassenger
                        .Where(bcp => bcp.IdCruise == idCruise))
                    .AsEnumerable();

                CruiseCriterion cruisecriterion = cruiseCriteria.GetCriteria(bookingCruisePassengers); // Récupération du critére le plus pertinent
                if (cruisecriterion != null)
                {
                    cruisecriterion = _shoreEntities.CruiseCriterion.Find(cruisecriterion.Id);
                }

                if (cruisecriterion == null) // Absence de critère conforme existant
                {
                    throw new ArgumentException("No criteria correspond to this cruise");
                }
                if (cruisecriterion.Survey.Language == null || cruisecriterion.Survey.Language.Count == 0) // Absence de langage pour le questionnaire associé à ce critére
                {
                    throw new ArgumentException("No language available for the associated survey");
                }

                foreach (Passenger p in passengers)
                {
                    GenerateToken(p);
                    BookingCruisePassenger bookingCruisePassenger = null;
                    string fileMergeName = null;
                    try
                    {
                        bookingCruisePassenger = _shoreEntities.BookingCruisePassenger
                            .First(bcp => bcp.IdCruise.Equals(idCruise) && bcp.IdPassenger.Equals(p.Id));

                        if (bookingCruisePassenger != null && !string.IsNullOrEmpty(p.Email))
                        {
                            List<string> attachementList = new List<string>();
                            Language language = null;
                            string mailFilePath = null;
                            string mailFilename = null;

                            switch (p.IdStatus) // traitement selon l'état du passager courrant
                            {
                                case Constants.NOT_APPLICABLE_NOT_APPLICABLE: // Envoi email non effectué
                                case Constants.SHORE_STATUS_QM_NOT_SENT: // Envoi email initial pour les QM non envoyé
                                case Constants.SHORE_STATUS_QM_SENT: // Envoi email initial pour les QM déjà envoyé
                                case Constants.SHORE_STATUS_QM_RECEIVED: // Envoi email initial pour les QM déjà reçu
                                    if (bookingCruisePassenger.Booking.IsGroup) // Traitement pour les passagers en groupe
                                    {
                                        language = (from l in cruisecriterion.Survey.Language
                                                    where !string.IsNullOrEmpty(l.GroupSurveyMail) &&
                                                   (l.IdLanguage.Equals(bookingCruisePassenger.Booking.IdLanguage) || l.IsDefault)
                                                    orderby l.IsDefault
                                                    select l).FirstOrDefault(); // Récuparation du langage pertinent

                                        mailFilePath = AppSettings.FolderMailGroup;
                                        mailFilename = language?.GroupSurveyMail;
                                    }
                                    else // Traitement pour les passagers individuel
                                    {
                                        language = (from l in cruisecriterion.Survey.Language
                                                    where !string.IsNullOrEmpty(l.IndividualSurveyMail) &&
                                                   (l.IdLanguage.Equals(bookingCruisePassenger.Booking.IdLanguage) || l.IsDefault)
                                                    orderby l.IsDefault
                                                    select l).FirstOrDefault(); // Récuparation du langage pertinent

                                        mailFilePath = AppSettings.FolderMailIndividual;
                                        mailFilename = language?.IndividualSurveyMail;
                                    }

                                    string filePathSurvey = FileManager.FileGetPath(AppSettings.FolderSurveyIndividual, language?.Id.ToString() + language?.IndividualSurveyFileName, false);
                                    if (!string.IsNullOrEmpty(filePathSurvey))
                                    {
                                        fileMergeName = Path.Combine(AppSettings.FolderTemp, bookingCruisePassenger.Booking.Number.ToString() + " - "
                                        + bookingCruisePassenger.Passenger.LastName + " " + bookingCruisePassenger.Passenger.FirstName + ".pdf");
                                        string fileMergePath = Path.Combine(AppSettings.FolderTemp, fileMergeName);
                                        Common.Pdf.MergePdf(filePathSurvey, fileMergePath, AppSettings.FolderTemp, bookingCruisePassenger);
                                        attachementList.Add(fileMergePath);
                                    }
                                    resultCounter.NbSendSurveyMail++;
                                    break;
                                default: // Autre état : continue la boucle en sautant le passagers courrant
                                    continue;
                            }

                            // Récupération du chemin de fichier modèle d'email   
                            mailFilePath = (!string.IsNullOrEmpty(mailFilename) && !string.IsNullOrEmpty(mailFilePath))
                                ? FileManager.FileGetPath(mailFilePath, language.Id.ToString() + mailFilename, false)
                                : null;

                            if (string.IsNullOrEmpty(mailFilePath))
                            {
                                throw new FileNotFoundException("language Id : " + language?.Id.ToString() + " mail file not found");
                            }

                            Storage.Message message = new Storage.Message(mailFilePath);
                            if (message != null)
                            {
                                // Envoi du message                               
                                using (message)
                                {
                                    Mail mail = new Mail()
                                    {
                                        Body = MailServer.ReplaceTags(message.BodyHtml, bookingCruisePassenger.Booking, bookingCruisePassenger, p),
                                        From = AppSettings.AddressFrom,
                                        Subject = MailServer.ReplaceTags(message.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, p),
                                        Attachments = attachementList.Count > 0 ? attachementList : null
                                    };

                                    // Création de la liste des destinataire
                                    MailServer.SendMailToRecipient(mail, new Recipient(p.FirstName + " " + p.LastName, p.Email), AppSettings.AddressDebug);
                                    if (additionnalRecipients != null)
                                    {
                                        foreach (KeyValuePair<string, string> recipient in additionnalRecipients)
                                        {
                                            if (!string.IsNullOrEmpty(recipient.Value))
                                            {
                                                MailServer.SendMailToRecipient(mail, new Recipient(recipient.Key, recipient.Value), AppSettings.AddressDebug);
                                            }
                                        }
                                    }
                                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Send, CurrentUser, "Send individual relaunch for passenger Id : " + p.Id.ToString(), (bookingCruisePassenger != null ? (int?)bookingCruisePassenger.Booking.Number : null));
                                }

                                // Maj passengers
                                if (!withoutPassengerSave)
                                {
                                    //Update QmReceived to Qm sent
                                    if (bookingCruisePassenger.Passenger.IdStatus == Constants.SHORE_STATUS_QM_RECEIVED && statusNeedToBeUpdate)
                                    {
                                        bookingCruisePassenger.Passenger.IdStatus = Constants.SHORE_STATUS_QM_SENT;
                                        bookingCruisePassenger.Passenger.AutoAttachment = null;
                                        resultCounter.NbUpdatePassengerStatus++;
                                    }
                                    int nbSent = bookingCruisePassenger.Passenger.SentCount + 1;
                                    bookingCruisePassenger.Passenger.IsEmailValid = MailServer.IsValidEmail(p.Email);
                                    bookingCruisePassenger.Passenger.SentCount = nbSent;
                                    bookingCruisePassenger.Passenger.SentDate = Now;
                                    bookingCruisePassenger.Passenger.ModificationDate = Now;
                                    bookingCruisePassenger.Passenger.Editor = CurrentUser;

                                    if (bookingCruisePassenger.Passenger.IdStatus == Constants.SHORE_STATUS_QM_NOT_SENT)
                                    {
                                        bookingCruisePassenger.Passenger.IdStatus = Constants.SHORE_STATUS_QM_SENT;
                                    }
                                    _shoreEntities.SaveChanges();
                                }

                                // Maj booking
                                bookingCruisePassenger.Booking.IdSurveyLanguage = language.Id;
                                bookingCruisePassenger.Booking.ModificationDate = Now;
                                bookingCruisePassenger.Booking.Editor = CurrentUser;
                                _shoreEntities.SaveChanges();
                            }
                        }
                        else
                        {
                            resultCounter.NbPassengersWithoutEmail++;
                            throw new InvalidDataException("Invalid email adress where found");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultCounter.NbErrorMail++;
                        LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Send, CurrentUser,
                            "Send individual relaunch for passenger Id : " + p.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")",
                            (bookingCruisePassenger != null ? (int?)bookingCruisePassenger.Booking.Number : null));
                    }
                    finally
                    {
                        // Suppression du questionnaire => Obligation de l'enregistrer sur le disque pour le mettre en pièce jointe du mail
                        FileManager.FileDelete(AppSettings.FolderTemp, fileMergeName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Send, CurrentUser,
                    "Send individual relaunch" + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
            return resultCounter;
        }
        #endregion

        #region OpenDocument
        /// <summary>
        /// Retourne un document donné
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <param name="saveSeen">Indique si le document doit être marquer comme lu</param>
        /// <returns>Chemin du document</returns>
        public string OpenDocument(int id, bool saveSeen)
        {
            string filePath = null;

            string CurrentUser = HttpContext.Current.User.Identity.Name;
            try
            {
                Document doc = _shoreEntities.Document.Find(id);
                filePath = FileManager.FileGetPath(GetDocumentPath(doc), doc.FileName, false);

                if (!string.IsNullOrEmpty(filePath))
                {
                    if (saveSeen)
                    {
                        doc.IdStatus = Constants.DOCUMENT_STATUS_SEEN;
                        _shoreEntities.SaveChanges();
                    }
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.File, LogManager.LogAction.Get, CurrentUser, "Get document Id : " + id.ToString());
                }
                else
                {
                    LogManager.InsertLog(LogManager.LogLevel.Warning, LogManager.LogType.File, LogManager.LogAction.Get, CurrentUser, "Document does not exist Id : " + id.ToString());
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.File, LogManager.LogAction.Get, CurrentUser,
                    "Get document Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }

            return filePath;
        }
        #endregion

        #region GetDocumentPath
        /// <summary>
        /// Renvoi le chemin d'accés à un document
        /// </summary>
        /// <param name="doc">document concerné</param>
        /// <returns>Chemin d'accés du document, null en cas d'erreur</returns>
        public string GetDocumentPath(Document doc, int? forceIdPassenger = null)
        {
            string path = null;
            if (doc.IdPassenger == 0)
            {
                path = (forceIdPassenger.HasValue)
                    ? Path.Combine(AppSettings.FolderPassenger, doc.ReceiptDate.Year + "-" + doc.ReceiptDate.Month.ToString().PadLeft(2, '0'), forceIdPassenger.ToString())
                    : AppSettings.FolderAvailable;
            }
            else
            {
                path = Path.Combine(AppSettings.FolderPassenger, doc.ReceiptDate.Year + "-" + doc.ReceiptDate.Month.ToString().PadLeft(2, '0'), doc.IdPassenger.ToString());
            }

            return path;
        }
        #endregion

        #region DetachDocument
        /// <summary>
        /// Detache un document de son passager
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        public void DetachDocument(int id)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            try
            {
                Document document = _shoreEntities.Document.Find(id);
                string filepath = GetDocumentPath(document);
                if (!string.IsNullOrEmpty(filepath))
                {
                    FileManager.FileMove(filepath, document.FileName, AppSettings.FolderAvailable, document.FileName);
                    FileManager.DirectoryDelete(filepath);
                }

                document.IdPassenger = 0;
                document.Editor = CurrentUser;
                document.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Document, LogManager.LogAction.Separate, CurrentUser, "Detach document Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Document, LogManager.LogAction.Separate, CurrentUser,
                    "Detach document Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Supprime un passager et les lignes associés
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        public void Delete(int idPassenger, int idCruise)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                BookingCruisePassenger bookingCruisePassenger = null;
                int? bookingNumber = null;
                try
                {
                    bookingCruisePassenger = _shoreEntities.BookingCruisePassenger
                        .First(bcp => bcp.IdPassenger.Equals(idPassenger) && bcp.IdCruise.Equals(idCruise));
                    bookingNumber = bookingCruisePassenger.Booking.Number;
                    List<string> listDirectoryPath = null;

                    int nbBookingCruisePassenger = (from bcp in _shoreEntities.BookingCruisePassenger
                                                    where bcp.IdPassenger.Equals(idPassenger)
                                                    select bcp.Id).Distinct().Count(); // Nombre de croisière pour le passager
                    int nbPassengerBooking = bookingCruisePassenger.Booking.BookingCruisePassenger.Distinct().Count(); // Nombre de passager dans le meme booking                  

                    if (nbBookingCruisePassenger <= 1)
                    {
                        listDirectoryPath = (from d in bookingCruisePassenger.Passenger.Document
                                             select (Path.Combine(d.ReceiptDate.Year + "-" + d.ReceiptDate.Month.ToString().PadLeft(2, '0'), d.IdPassenger.ToString())))
                                             .Distinct()
                                             .ToList();
                        _shoreEntities.Information.RemoveRange(bookingCruisePassenger.Passenger.Information);
                        _shoreEntities.Document.RemoveRange(bookingCruisePassenger.Passenger.Document);
                        _shoreEntities.BookingActivity.RemoveRange(bookingCruisePassenger.Passenger.BookingActivity);
                        _shoreEntities.Passenger.Remove(bookingCruisePassenger.Passenger);
                    }

                    if (nbPassengerBooking <= 1)
                    {
                        _shoreEntities.BookingActivity.RemoveRange(from ba in _shoreEntities.BookingActivity
                                                                   where ba.IdBooking.Equals(bookingCruisePassenger.Booking.Id)
                                                                   select ba);
                    }

                    _shoreEntities.BookingCruisePassenger.Remove(bookingCruisePassenger);
                    _shoreEntities.SaveChanges();

                    if (listDirectoryPath != null)
                    {
                        foreach (string directoryPath in listDirectoryPath)
                        {
                            FileManager.DirectoryDelete(Path.Combine(AppSettings.FolderPassenger, directoryPath), true);
                        }
                    }

                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name,
                        "Delete passenger Id : " + idPassenger.ToString(),
                        bookingNumber.Value);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name,
                        "Delete passenger Id : " + idPassenger.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")",
                        (bookingNumber.HasValue ? (int?)bookingNumber.Value : null));
                    throw;
                }
            }
        }
        #endregion

        #region private

        #region GenerateToken
        private Passenger GenerateToken(Passenger p)
        {
            try
            {
                if (String.IsNullOrEmpty(p.Token))
                {
                    Guid guid = Guid.NewGuid();
                    p.Token = guid.ToString();
                    _shoreEntities.SaveChanges();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit passenger Id : " + p.Id.ToString());
                }
                return p;
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit passenger Id :  " + p.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #endregion
    }
}