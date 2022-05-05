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
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;

    #region Modèles des vues

    #region GetMessageViewModel
    /// <summary>
    /// Représentation pour l'affichage du message par email
    /// </summary>
    public class GetMessageViewModel
    {
        /// <summary>
        /// Identifiant du document
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du passager
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Message de l'email
        /// </summary>
        public string Message { get; set; }
    }
    #endregion

    #region LinkViewModel
    /// <summary>
    /// Représentation pour la liaison des document à un passagers
    /// </summary>
    public class LinkViewModel
    {
        /// <summary>
        /// Liste d'identifiant des document a attaché 
        /// </summary>
        [Required(ErrorMessage = "No files selected")]
        public List<int> IdsDocument { get; set; }

        /// <summary>
        /// Nom des documents
        /// </summary>
        public string DocumentsNames { get; set; }

        /// <summary>
        /// Identifiant du passager 
        /// </summary>
        [Required(ErrorMessage = "Passenger must be selected")]
        public int? IdPassenger { get; set; }
    }
    #endregion

    #endregion

    #region Gestion des document par document
    /// <summary>
    /// Classe de gestion des document par document
    /// </summary>
    public class AvailableDocumentClass : SharedPassengerClass
    {
        #region Properties & Constructors

        public AvailableDocumentClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region GetMessageDocument
        /// <summary>
        /// Retourne une representation du document pour l'affichage du message
        /// </summary>
        /// <param name="id">Identifiant du document/param>
        /// <returns>Une représentation pour le document</returns>
        public GetMessageViewModel GetMessageDocument(int id)
        {
            GetMessageViewModel model = new GetMessageViewModel();

            Document document = _shoreEntities.Document.Find(id);
            if (document != null)
            {
                model.Id = document.Id;
                model.Message = document.Message;
                model.Name = "Unknown passenger";

                if (document.IdPassenger != 0)
                {
                    model.Name = document.Passenger.FirstName + " " + document.Passenger.LastName;
                }
            }
            return model;
        }
        #endregion

        #region GetLinkModel
        /// <summary>
        /// Récupére une representation des documennt pour la liaison avec un passager
        /// </summary>
        /// <param name="idsDocument">Liste des document à attaché</param>
        /// <returns>Représentation des documents</returns>
        public LinkViewModel GetLinkModel(List<int> idsDocument)
        {
            LinkViewModel model = new LinkViewModel
            {
                IdsDocument = idsDocument,
                DocumentsNames = ((from doc in _shoreEntities.Document where idsDocument.Contains(doc.Id) select doc.Name).ToList()).Aggregate((current, next) => current + ", " + next)
            };

            return model;
        }
        #endregion

        #region Link
        /// <summary>
        /// Lie un document à un passager
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <param name="idPassenger">Identifiant du passager</param>
        public void Link(int idDocument, int idPassenger)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                try
                {
                    Document document = _shoreEntities.Document.Find(idDocument);

                        if (document.IdPassenger != 0 && document.IdPassenger != idPassenger)
                        {
                            throw new ArgumentException("At least one file is already associated with another passenger");
                        }

                    if (document.IdPassenger == 0)
                    {
                        Passenger passenger = _shoreEntities.Passenger.Find(idPassenger);

                        string filepath = GetDocumentPath(document);
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            string destPath = GetDocumentPath(document, idPassenger);
                            FileManager.FileMove(filepath, document.FileName, destPath, document.FileName);
                            FileManager.DirectoryDelete(filepath);
                        }
                        document.IdPassenger = idPassenger;
                        document.Editor = CurrentUser;
                        document.ModificationDate = Now;
                        // Set AutoAttachment to false
                        passenger.AutoAttachment = false;
                        _shoreEntities.SaveChanges();

                        switch (document.Passenger.IdAdvice)
                        {
                            case Constants.NOT_APPLICABLE_NOT_APPLICABLE:
                                document.Passenger.IdStatus = Constants.SHORE_STATUS_QM_RECEIVED;
                                document.Passenger.Editor = CurrentUser;
                                document.Passenger.ModificationDate = DateTime.Now;
                                break;
                            case Constants.ADVICE_WAITING_FOR_CLARIFICATION:
                                document.Passenger.IdStatus = Constants.SHORE_STATUS_QM_NEW_DOCUMENTS;
                                document.Passenger.Editor = CurrentUser;
                                document.Passenger.ModificationDate = DateTime.Now;
                                break;
                        }
                        _shoreEntities.SaveChanges();
                        dbContextTransaction.Commit();
                        SendMailLink(idPassenger);
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Document, LogManager.LogAction.Associate, CurrentUser, "Link document Id : " + idDocument.ToString() + " to passenger Id : " + idPassenger.ToString());
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Document, LogManager.LogAction.Associate, CurrentUser, "Link document Id : " + idDocument.ToString() + " to passenger Id : " + idPassenger.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un document
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        public void Delete(int id)
        {
            try
            {
                Document document = _shoreEntities.Document.Find(id);
                string filepath = GetDocumentPath(document);
                if (!string.IsNullOrEmpty(filepath))
                {
                    FileManager.FileDelete(filepath, document.FileName);
                    FileManager.DirectoryDelete(filepath);
                }

                _shoreEntities.Document.Remove(document);
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Document, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete document Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Document, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete document Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region SendMailLink
        /// <summary>
        /// Envoi un email de confirmation au passager lors d'une affectation manuelle
        /// </summary>
        /// <param name="idPassenger"></param>
        public void SendMailLink(int idPassenger)
        {
            //Recupere le langage associé a l id du passager
            BookingCruisePassenger bookingCruisePassenger = (from p in _shoreEntities.Passenger
                                                             join bcp in _shoreEntities.BookingCruisePassenger on p.Id equals bcp.IdPassenger
                                                             join boo in _shoreEntities.Booking on bcp.IdBooking equals boo.Id
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
                languageMail = (from lang in _shoreEntities.Language
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
                languageMail = (from lang in _shoreEntities.Language
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
            if (!string.IsNullOrWhiteSpace(mailModel) && File.Exists(mailModel))
            {
                if (!string.IsNullOrEmpty(bookingCruisePassenger.Booking.IsGroup 
                    ? languageMail.GroupAutomaticResponse 
                    : languageMail.IndividualAutomaticResponse))
                {
                    using (Storage.Message messageAuto = new Storage.Message(mailModel))
                    {
                        Mail mail = new Mail()
                        {
                            Body = string.Concat(MailServer.ReplaceTags(messageAuto.BodyHtml, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger), "#", bookingCruisePassenger.Passenger.Id),
                            From = AppSettings.AddressNoReply,
                            Subject = string.Concat("#", bookingCruisePassenger.Passenger.Id, " - ", MailServer.ReplaceTags(messageAuto.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger))
                        };

                        MailServer.SendMailLinkPassengerToDocument(mail, new Recipient("", bookingCruisePassenger.Booking.IsGroup ? AppSettings.GroupFrom : bookingCruisePassenger.Passenger.Email), AppSettings.AddressDebug);
                    }
                }
            }
        }
        #endregion

    }
    #endregion
}