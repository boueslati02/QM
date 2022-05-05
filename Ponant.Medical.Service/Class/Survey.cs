using MsgReader.Outlook;
using Ponant.Medical.Common;
using Ponant.Medical.Common.MailServer;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ponant.Medical.Service.Class
{
    /// <summary>
    /// Classe de gestion du nettoyage des documents médicaux
    /// </summary>
    public class Survey
    {
        #region Associate
        /// <summary>
        /// Associe les questionnaires aux passagers
        /// </summary>
        public void Associate()
        {
            try
            {
                MailServer mailServer = new MailServer(AppSettings.AddressFrom);

                if (mailServer != null)
                {
                    List<MailMessage> messages = mailServer.Receive(AppSettings.FolderSandBox, AppSettings.FolderPassenger);

                    if (messages != null)
                    {
                        foreach (MailMessage message in messages)
                        {
                            try
                            {
                                if (message.Attachments != null)
                                {
                                    using (ShoreEntities db = new ShoreEntities())
                                    {
                                        using (DbContextTransaction transaction = db.Database.BeginTransaction())
                                        {
                                            bool isAttachmentExisted = true;

                                            //Ce compteur permet de connaitre le nb de lignes en bdd
                                            int cptLigneBdd = message.Attachments.Count;
                                            List<int> passengerIdAlreadyUpdate = new List<int>();

                                            foreach (MailAttachment attachment in message.Attachments)
                                            {
                                                int idPassenger = attachment.IdPassenger ?? message.IdPassenger;

                                                try
                                                {
                                                    Document document = new Document
                                                    {
                                                        CreationDate = DateTime.Now,
                                                        Creator = AppSettings.UserAccount,
                                                        Editor = AppSettings.UserAccount,
                                                        Email = message.Email,
                                                        FileName = attachment.FileName,
                                                        IdPassenger = idPassenger,
                                                        IdStatus = Constants.DOCUMENT_STATUS_NOT_SEEN,
                                                        Message = message.Message,
                                                        ModificationDate = DateTime.Now,
                                                        Name = attachment.Name,
                                                        ReceiptDate = message.ReceivedDate
                                                    };

                                                    db.Document.Add(document);

                                                    string details = document.IdPassenger == 0
                                                        ? string.Format("Add filename : {0} from {1} to sandbox", attachment.Name, message.Email)
                                                        : string.Format("Add filename : {0} from {1} to passenger {2}", attachment.Name, message.Email, document.IdPassenger);

                                                    string path = (document.IdPassenger == 0)
                                                        ? Path.Combine(AppSettings.FolderSandBox)
                                                        : Path.Combine(AppSettings.FolderPassenger, document.ReceiptDate.ToString("yyyy-MM"), document.IdPassenger.ToString());
                                                    path = Path.Combine(path, document.FileName);

                                                    if (!File.Exists(path))
                                                    {
                                                        isAttachmentExisted = false;
                                                    }

                                                    if (!idPassenger.Equals(0))
                                                    {
                                                        Passenger passenger = db.Passenger.Single(p => p.Id.Equals(idPassenger));
                                                        passenger.IdStatus = passenger.IdStatus.Equals(Constants.SHORE_STATUS_QM_INCOMPLETE)
                                                            ? Constants.SHORE_STATUS_QM_NEW_DOCUMENTS
                                                            : Constants.SHORE_STATUS_QM_RECEIVED;
                                                        passenger.Editor = AppSettings.UserAccount;
                                                        passenger.ModificationDate = DateTime.Now;
                                                        passenger.AutoAttachment = true;

                                                        if (!passengerIdAlreadyUpdate.Contains(idPassenger))
                                                        {
                                                            passengerIdAlreadyUpdate.Add(idPassenger);
                                                            cptLigneBdd += 1;
                                                        }
                                                    }

                                                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Associate, AppSettings.UserAccount, details);
                                                }
                                                catch (Exception exception)
                                                {
                                                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, AppSettings.UserAccount, "Passenger Id : " + idPassenger.ToString() + " Attachement Name : " + attachment.Name
                                                        + " (" + string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
                                                }
                                            }

                                            int nbLigneSave = db.SaveChanges();

                                            // permet de tester si les fichiers sont enregistres physiquement et si le nb de lignes enregistrees correspondent 
                                            if (isAttachmentExisted && cptLigneBdd == nbLigneSave)
                                            {
                                                //Si le document est bien enregistré en physique/base de donnée alors supprimé
                                                transaction.Commit();
                                                mailServer.Delete(message.IdMessage);
                                            }
                                            else // Dans le cas ou il y a une piece jointe qui n'existe pas ou que les lignes de la base de donnee ne correspondent pas
                                            {
                                                transaction.Rollback();
                                                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, AppSettings.UserAccount,
                                                    "Backtracking for the addition of associated documents due to a difference in the verification counter");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Associate, AppSettings.UserAccount, string.Concat(exception.Message, exception.InnerException != null ? " || " + exception.InnerException.Message : null) + ")");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Survey, LogManager.LogAction.Associate, AppSettings.UserAccount, exception);
            }
        }
        #endregion

        #region Clean
        /// <summary>
        /// Nettoyage des documents médicaux du bac à sable de plus 3 mois.
        /// </summary>
        public void Clean()
        {
            using (ShoreEntities db = new ShoreEntities())
            {
                // Récupération des documents reçus depuis plus de 3 mois
                DateTime from = DateTime.Today.AddMonths(AppSettings.SandBoxCleaningInterval);
                List<Document> documents = db.Document.Where(s => s.ReceiptDate < from && s.IdPassenger.Equals(Constants.NOT_APPLICABLE_NOT_APPLICABLE)).ToList();

                foreach (Document document in documents)
                {
                    // Suppression en base
                    db.Document.Remove(document);
                    db.SaveChanges();

                    // Suppression physique
                    string path = Path.Combine(AppSettings.FolderSandBox, document.FileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    // Log
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Delete, AppSettings.UserAccount, string.Format("Cleaning sandbox document : {0}", document.Name), null);
                }
            }
        }
        #endregion

        #region Sent
        /// <summary>
        /// Envoit les questionnaires médicaux aux passagers  
        /// </summary>
        /// <param name="bookingNumber">Numéro de booking à traiter</param>
        /// <param name="isRelaunch">L'envoi provient d'une relance</param>
        /// <param name="IsLangageUpdated"> Modification de la langue du booking dans le fichier xml</param>
        /// <returns>Vrai si le questionnaire a été envoyé, faux sinon</returns>
        public async Task<bool> Sent(int bookingNumber, bool isRelaunch, bool IsLanguageUpdated)
        {
            bool isSent = false;
            int idLanguage = 0;

            try
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    Data.Shore.Booking booking = db.Booking.Single(b => b.Number.Equals(bookingNumber));
                    CruiseCriteria cruiseCriteria = new CruiseCriteria(db);
                    CruiseCriterion criteria = cruiseCriteria.GetCriteria(booking.BookingCruisePassenger);
                    int? idCruise = GetIdCruise(booking, criteria);
                    Language language = null;

                    if (criteria != null && idCruise.HasValue)
                    {
                        List<BookingCruisePassenger> bookingCruisePassengers = booking.BookingCruisePassenger
                            .Where(bp => bp.IsEnabled && bp.IdCruise.Equals(idCruise) && (criteria.Activity == null ||
                                bp.Booking.BookingActivity.Any(ba => criteria.Activity.Contains(ba.LovActivity.Code) && ba.IdBooking == bp.IdBooking && ba.IdPassenger.Equals(bp.IdPassenger))))
                            .ToList();

                        foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisePassengers)
                        {
                            isSent = false;
                            bool toSent = false;
                            bool sentForLanguageUpdate = false;

                            if (IsLanguageUpdated)
                            {
                                // permet de vérifier si la croisière n'est pas déjà passée et si le QM n'est pas envoyé
                                if (DateTime.Compare(DateTime.Now.Date, bookingCruisePassenger.Cruise.SailingDate.Date) < 0)
                                {
                                    // car on renvoie un mail dans la nouvelle langue si il n'est pas envoyé ou si il est envoyé mais dans la mauvaise langue
                                    if (bookingCruisePassenger.Passenger.IdStatus == Constants.SHORE_STATUS_QM_NOT_SENT || bookingCruisePassenger.Passenger.IdStatus == Constants.SHORE_STATUS_QM_SENT)
                                    {
                                        sentForLanguageUpdate = true;
                                    }
                                }
                            }

                            // permet de vérifier si la croisière n'est pas déjà passée
                            if (DateTime.Compare(DateTime.Now.Date, bookingCruisePassenger.Cruise.SailingDate.Date) < 0)
                            {
                                // Gere les criteres d'envoi du QM
                                if ((!string.IsNullOrEmpty(bookingCruisePassenger.Passenger.Email) || bookingCruisePassenger.Booking.IsGroup) &&
                                (bookingCruisePassenger.Passenger.SentCount.Equals(0) || sentForLanguageUpdate ||
                                bookingCruisePassenger.Passenger.IdStatus.Equals(Constants.SHORE_STATUS_QM_NOT_SENT) ||
                                (bookingCruisePassenger.Passenger.IdStatus.Equals(Constants.SHORE_STATUS_QM_SENT) && isRelaunch) ||
                                (bookingCruisePassenger.Passenger.IdStatus.Equals(Constants.SHORE_STATUS_QM_INCOMPLETE) && isRelaunch)))
                                {
                                    toSent = true;
                                }
                            }

                            if (toSent)
                            {
                                bookingCruisePassenger.Passenger.IsEmailValid = !string.IsNullOrEmpty(bookingCruisePassenger.Passenger.Email) && MailServer.IsValidEmail(bookingCruisePassenger.Passenger.Email);

                                if (bookingCruisePassenger.Passenger.IsEmailValid.Value || bookingCruisePassenger.Booking.IsGroup)
                                {
                                    // S'il existe un questionnaire dans la langue du booking, on utilise celui-là
                                    // Sinon on utilise le questionnaire de la langue par défaut
                                    if (bookingCruisePassenger.Booking.IsGroup)
                                    {
                                        language = (from l in criteria.Survey.Language
                                                    where !string.IsNullOrEmpty(l.GroupSurveyFileName) &&
                                                   (l.IdLanguage.Equals(booking.IdLanguage) || l.IsDefault)
                                                    orderby l.IsDefault
                                                    select l).FirstOrDefault();
                                    }
                                    else
                                    {
                                        language = (from l in criteria.Survey.Language
                                                    where !string.IsNullOrEmpty(l.IndividualSurveyFileName) &&
                                                   (l.IdLanguage.Equals(booking.IdLanguage) || l.IsDefault)
                                                    orderby l.IsDefault
                                                    select l).FirstOrDefault();
                                    }

                                    if (language != null)
                                    {
                                        idLanguage = language.Id;

                                        string filename = Path.Combine(bookingCruisePassenger.Booking.IsGroup ? AppSettings.FolderSurveyGroup : AppSettings.FolderSurveyIndividual,
                                                                       language.Id +
                                                                      (bookingCruisePassenger.Booking.IsGroup ? language.GroupSurveyFileName : language.IndividualSurveyFileName));

                                        if (File.Exists(filename))
                                        {
                                            string mergedPdfFilename = StringHelper.CleanInvalidChar(string.Concat(bookingCruisePassenger.Booking.Number.ToString(), " - ",
                                                bookingCruisePassenger.Passenger.LastName, " ", bookingCruisePassenger.Passenger.FirstName, ".pdf"));
                                            string mergedPdfPath = Path.Combine(AppSettings.FolderTemp, mergedPdfFilename);
                                            Common.Pdf.MergePdf(filename, mergedPdfPath, AppSettings.FolderTemp, bookingCruisePassenger);

                                            // Envoi du mail
                                            if (!string.IsNullOrEmpty(bookingCruisePassenger.Booking.IsGroup ? language.GroupSurveyMail : language.IndividualSurveyMail))
                                            {
                                                // Recherche du modèle de mail => S'il existe un questionnaire dans la langue du booking, on utilise celui-là
                                                // Sinon on utilise le questionnaire de la langue par défaut
                                                if (bookingCruisePassenger.Booking.IsGroup)
                                                {
                                                    if (isRelaunch)
                                                    {
                                                        language = (from l in criteria.Survey.Language
                                                                    where (l.IdLanguage.Equals(booking.IdLanguage) || l.IsDefault)
                                                                    orderby l.IsDefault
                                                                    select l).FirstOrDefault();
                                                    }
                                                    else
                                                    {
                                                        language = (from l in criteria.Survey.Language
                                                                    where !string.IsNullOrEmpty(l.GroupSurveyMail) &&
                                                                   (l.IdLanguage.Equals(booking.IdLanguage) || l.IsDefault)
                                                                    orderby l.IsDefault
                                                                    select l).FirstOrDefault();
                                                    }
                                                }
                                                else
                                                {

                                                    language = (from l in criteria.Survey.Language
                                                                where !string.IsNullOrEmpty(l.IndividualSurveyMail) &&
                                                               (l.IdLanguage.Equals(booking.IdLanguage) || l.IsDefault)
                                                                orderby l.IsDefault
                                                                select l).FirstOrDefault();
                                                }

                                                if (language != null)
                                                {
                                                    string mailModel = Path.Combine(bookingCruisePassenger.Booking.IsGroup ? AppSettings.FolderMailGroup : AppSettings.FolderMailIndividual,
                                                        language.Id + (bookingCruisePassenger.Booking.IsGroup ? language.GroupSurveyMail : language.IndividualSurveyMail));

                                                    if (File.Exists(mailModel))
                                                    {
                                                        using (Storage.Message message = new Storage.Message(mailModel))
                                                        {
                                                            Mail mail = new Mail()
                                                            {
                                                                Attachments = new List<string> { mergedPdfPath },
                                                                Body = string.Concat(MailServer.ReplaceTags(message.BodyHtml, booking, bookingCruisePassenger, bookingCruisePassenger.Passenger), "#", bookingCruisePassenger.Passenger.Id),
                                                                From = criteria.Survey.MedicalAdvice ? AppSettings.AddressFrom : AppSettings.AddressNoReply,
                                                                Subject = string.Concat("#", bookingCruisePassenger.Passenger.Id, " - ", MailServer.ReplaceTags(message.Subject, booking, bookingCruisePassenger, bookingCruisePassenger.Passenger))
                                                            };

                                                            MailServer.SendMailToRecipient(mail, new Recipient("", bookingCruisePassenger.Booking.IsGroup ? AppSettings.GroupFrom : bookingCruisePassenger.Passenger.Email), AppSettings.AddressDebug);
                                                            if ((!bookingCruisePassenger.Booking.IsGroup) && (booking.Agency != null) && (!string.IsNullOrWhiteSpace(booking.Agency.Email)))
                                                            {
                                                                MailServer.SendMailToRecipient(mail, new Recipient(booking.Agency.Name, booking.Agency.Email), AppSettings.AddressDebug);
                                                            }

                                                            isSent = true;
                                                            booking.IdSurveyLanguage = idLanguage;
                                                        }
                                                    }

                                                    // MAJ du passager
                                                    bookingCruisePassenger.Passenger.IdStatus = Constants.SHORE_STATUS_QM_SENT;
                                                    bookingCruisePassenger.Passenger.SentCount = bookingCruisePassenger.Passenger.SentCount + 1;
                                                    bookingCruisePassenger.Passenger.SentDate = DateTime.Now;

                                                    // Log
                                                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Send, AppSettings.UserAccount, string.Format("{0} envoyé à {1} {2}", language.IndividualSurveyFileName, bookingCruisePassenger.Passenger.LastName, bookingCruisePassenger.Passenger.FirstName), booking.Number);

                                                    // Suppression du questionnaire => Obligation de l'enregistrer sur le disque pour le mettre en pièce jointe du mail
                                                    if (File.Exists(mergedPdfPath))
                                                    {
                                                        File.Delete(mergedPdfPath);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!isSent && bookingCruisePassenger.Passenger.IdStatus.Equals(Constants.NOT_APPLICABLE_NOT_APPLICABLE))
                            {
                                bookingCruisePassenger.Passenger.IdStatus = Constants.SHORE_STATUS_QM_NOT_SENT;
                            }

                            bookingCruisePassenger.Passenger.ModificationDate = DateTime.Now;
                            bookingCruisePassenger.Passenger.Editor = AppSettings.UserAccount;

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.Survey, LogManager.LogAction.Send, AppSettings.UserAccount, exception);
            }

            return isSent;
        }
        #endregion

        #region GetIdCruise
        /// <summary>
        /// Obtient l'identifiant de la 1ère croisière qui répond aux critères
        /// </summary>
        /// <param name="booking">Instance de la réservation</param>
        /// <param name="criteria">Instance du critère</param>
        /// <returns>L'identifiant de la 1ère croisière qui répond aux critères</returns>
        public int? GetIdCruise(Data.Shore.Booking booking, CruiseCriterion criteria)
        {
            int? idCruise = null;

            BookingCruisePassenger bookingCruisePassenger = booking.BookingCruisePassenger.FirstOrDefault(bcp => (criteria.IdCruiseType == 0 || bcp.Cruise.IdTypeCruise.Equals(criteria.IdCruiseType)) &&
                                                                                                         (criteria.CruiseCriterionDestination.Count == 0 || criteria.CruiseCriterionDestination.Any(ccd => ccd.IdDestination.Equals(bcp.Cruise.IdDestination))) &&
                                                                                                         (criteria.CruiseCriterionShip.Count == 0 || criteria.CruiseCriterionShip.Any(ccs => ccs.IdShip.Equals(bcp.Cruise.IdShip))) &&
                                                                                                         (!criteria.Length.HasValue || bcp.Cruise.SailingLengthDays > criteria.Length.Value) &&
                                                                                                         (criteria.Cruise == null || bcp.Cruise.Code.Contains(criteria.Cruise)) &&
                                                                                                         (criteria.Activity == null || bcp.Booking.BookingActivity.Any(ba => criteria.Activity.Contains(ba.LovActivity.Code))));

            if (bookingCruisePassenger == null || bookingCruisePassenger.Cruise == null)
            {
                bookingCruisePassenger = booking.BookingCruisePassenger.FirstOrDefault();
            }

            idCruise = bookingCruisePassenger.Cruise.Id;

            return idCruise;
        }
        #endregion

        #region SendMailLink
        /// <summary>
        /// Envoi un email de confirmation au passager lors d'une affectation automatique
        /// </summary>
        /// <param name="idPassenger"></param>
        public static void SendMailLink(int idPassenger)
        {
            if (idPassenger != 0)
            {
                using (ShoreEntities db = new ShoreEntities())
                {
                    //Recupere le langage associé a l id du passager
                    BookingCruisePassenger bookingCruisePassenger = (from p in db.Passenger
                                                                     join bcp in db.BookingCruisePassenger on p.Id equals bcp.IdPassenger
                                                                     join boo in db.Booking on bcp.IdBooking equals boo.Id
                                                                     where (p.Id == idPassenger)
                                                                     select bcp).First();

                    // 1: Récuperer la table langage en fct du destinataire du message
                    Language languageMail;

                    //On récupère le chemin complet du template de message (individuel ou auto)
                    string mailModel = null;

                    //Permet de récuperer le langage en fonction, de la langue par défaut, ou de la réservation
                    if (bookingCruisePassenger.Booking.IsGroup)
                    {
                        //groupAutomaticResponse
                        languageMail = (from lang in db.Language
                                        where !string.IsNullOrEmpty(lang.GroupAutomaticResponse) &&
                                        (lang.IdLanguage == bookingCruisePassenger.Booking.IdLanguage || lang.IsDefault)
                                        orderby lang.IsDefault
                                        select lang).FirstOrDefault();

                        if (languageMail != null)
                        {
                            mailModel = Path.Combine(AppSettings.FolderMailGroupAutomaticResponse, languageMail.Id + languageMail.GroupAutomaticResponse);
                        }
                    }
                    else
                    {
                        //individualAutomaticResponse
                        languageMail = (from lang in db.Language
                                        where !string.IsNullOrEmpty(lang.IndividualAutomaticResponse) &&
                                        (lang.IdLanguage == bookingCruisePassenger.Booking.IdLanguage || lang.IsDefault)
                                        orderby lang.IsDefault
                                        select lang).FirstOrDefault();

                        if (languageMail != null)
                        {
                            mailModel = Path.Combine(AppSettings.FolderMailIndividualAutomaticResponse, languageMail.Id + languageMail.IndividualAutomaticResponse);
                        }
                    }

                    //Envoi du mail
                    if (!string.IsNullOrWhiteSpace(mailModel)
                        && File.Exists(mailModel)
                        && !string.IsNullOrEmpty(bookingCruisePassenger.Booking.IsGroup ? languageMail.GroupAutomaticResponse : languageMail.IndividualAutomaticResponse))
                    {
                        using (Storage.Message messageAuto = new Storage.Message(mailModel))
                        {
                            Mail mail = new Mail()
                            {
                                Body = string.Concat(MailServer.ReplaceTags(messageAuto.BodyHtml, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger), "#", bookingCruisePassenger.Passenger.Id),
                                From = AppSettings.AddressNoReply,
                                Subject = string.Concat("#", bookingCruisePassenger.Passenger.Id, " - ", MailServer.ReplaceTags(messageAuto.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger))
                            };

                            MailServer.SendMailToRecipient(mail, new Recipient("", bookingCruisePassenger.Booking.IsGroup ? AppSettings.GroupFrom : bookingCruisePassenger.Passenger.Email), AppSettings.AddressDebug);
                        }
                    }
                }
            }
        }
        #endregion
    }
}