namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
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
    using System.Web.Mvc;

    #region Modèles des vues

    #region AddDocumentViewModel
    /// <summary>
    /// Représentation pour l'affichage d'ajout d'un nouveau document pour un passager
    /// </summary>
    public class AddDocumentViewModel
    {
        /// <summary>
        /// Identifiant du passager
        /// </summary>
        [Required]
        public int IdPassenger { get; set; }

        /// <summary>
        /// Nom et prénom du passager
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nom du fichier à ajouter
        /// </summary>
        [Required]
        [Display(Name = "Document")]
        [MaxLength(100)]
        public string PassengerDocumentName { get; set; }
    }
    #endregion

    #region CreatePassengerDocumentViewModel
    public class CreatePassengerDocumentViewModel
    {
        /// <summary>
        /// Code de la croisière
        /// </summary>
        [Required]
        [Display(Name = "Cruise")]
        [Remote("CheckCruiseValue", "PassengerDocument", ErrorMessage = "This cruise does not exist", HttpMethod = "GET")]
        public string CruiseCode { get; set; }

        /// <summary>
        /// Code du booking
        /// </summary>
        [Required]
        [Display(Name = "N° booking")]
        [Remote("CheckBookingValue", "PassengerDocument", AdditionalFields = "CruiseCode ", ErrorMessage = "This booking does not exist for this cruise", HttpMethod = "GET")]
        public string BookingCode { get; set; }

        [Required]
        [Display(Name = "LastName")]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "UsualName")]
        [MaxLength(64)]
        public string UsualName { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        [MaxLength(64)]
        public string FirstName { get; set; }

        /// <summary>
        /// Date de naissance du passager
        /// </summary>
        [Display(Name = "BirthDate")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Nom du fichier à ajouter
        /// </summary>
        [Required]
        [Display(Name = "Document")]
        [MaxLength(100)]
        public string PassengerDocumentName { get; set; }
    }

    #endregion

    #endregion

    #region Gestion des documents par passager
    /// <summary>
    /// Classe de gestion des documents par passager
    /// </summary>
    public class PassengerDocumentClass : SharedPassengerClass
    {
        #region Properties & Constructor
        private const string _PassengerDocumentName = "_PassengerDocumentName";

        public PassengerDocumentClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region GetPassengerDocuments
        /// <summary>
        /// Retourne la liste des documents associé au passager
        /// </summary>
        /// <param name="id">Identifiant du passager/param>
        /// <returns>La liste des document du passager, null en cas d'erreur</returns>
        public List<Document> GetPassengerDocuments(int id)
        {
            Passenger passenger = _shoreEntities.Passenger.Find(id);
            if (passenger.Document != null
                && passenger.Document.Any())
            {
                return passenger.Document.ToList();
            }

            return null;
        }
        #endregion

        #region GetPassengerForAddDocument
        /// <summary>
        /// Retourne un passager donné
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <returns>un passager, null en cas d'erreur</returns>
        public AddDocumentViewModel GetPassengerForAddDocument(int idPassenger)
        {
            AddDocumentViewModel model = new AddDocumentViewModel();

            Passenger passenger = _shoreEntities.Passenger.Find(idPassenger);
            model.IdPassenger = idPassenger;
            model.Name = passenger.FirstName + " " + passenger.LastName;

            return model;
        }
        #endregion

        #region GetDocument
        /// <summary>
        /// Retourne un documents donné
        /// </summary>
        /// <param name="idDocument"> Identifiant du document</param>
        /// <returns>un document, null en cas d'erreur</returns>
        public Document GetDocument(int idDocument)
        {
            Document document = _shoreEntities.Document.Find(idDocument);
            return document;
        }
        #endregion

        #region AddDocument
        public void AddDocument(AddDocumentViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            string fileName = FileManager.ReplaceCharFilename(CurrentUser + _PassengerDocumentName + model.PassengerDocumentName);
            AddDocumentCommon(model, fileName, _PassengerDocumentName, CurrentUser);
        }

        public void AddDocument(AddDocumentViewModel model, string fileInputName)
        {
            Passenger passenger = _shoreEntities.Passenger.Find(model.IdPassenger);
            string CurrentUser = string.Concat(passenger.FirstName, passenger.LastName);
            string fileName = FileManager.ReplaceCharFilename(CurrentUser + fileInputName + model.PassengerDocumentName);
            AddDocumentCommon(model, fileName, fileInputName, CurrentUser);
        }
        #endregion

        #region AddPassenger
        /// <summary>
        /// Ajoute un nouveau passager
        /// </summary>
        /// <param name="model">Model d'ajout d'un passager</param>
        public void AddPassenger(CreatePassengerDocumentViewModel model)
        {

            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                BookingCruisePassenger bookingCruisePassenger = null;
                try
                {
                    Passenger passenger = new Passenger
                    {
                        LastName = model.LastName,
                        UsualName = model.UsualName,
                        FirstName = model.FirstName,
                        IsEmailUpdated = false,
                        IsEmailValid = false,
                        SentCount = 0,
                        IsExtract = false,
                        Creator = CurrentUser,
                        CreationDate = Now,
                        Editor = CurrentUser,
                        ModificationDate = Now,
                        IdStatus = Constants.SHORE_STATUS_QM_RECEIVED,
                        IdTitle = 0,
                        BirthDate = model.BirthDate,
                        AutoAttachment = false,
                        IsDownloaded = false
                    };

                    _shoreEntities.Passenger.Add(passenger);
                    _shoreEntities.SaveChanges();

                    // Ajout du booking cruise passenger
                    bookingCruisePassenger = new BookingCruisePassenger
                    {
                        CabinNumber = null,
                        IsEnabled = true,
                        Creator = CurrentUser,
                        CreationDate = Now,
                        Editor = CurrentUser,
                        ModificationDate = Now,
                        IdBooking = (from b in _shoreEntities.Booking where b.Number.ToString().Equals(model.BookingCode) select b.Id).Single(),
                        IdCruise = (from c in _shoreEntities.Cruise where c.Code.ToString().Equals(model.CruiseCode) select c.Id).Single(),
                        IdPassenger = passenger.Id
                    };
                    _shoreEntities.BookingCruisePassenger.Add(bookingCruisePassenger);
                    _shoreEntities.SaveChanges();

                    // Ajout du document
                    Document document = FillDocument(passenger.Id, model.PassengerDocumentName, Now, CurrentUser, _PassengerDocumentName);
                    _shoreEntities.Document.Add(document);
                    _shoreEntities.SaveChanges();
                    
                    MoveDocumentFile(ref document, CurrentUser, _PassengerDocumentName);
                    _shoreEntities.SaveChanges();

                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Add, CurrentUser, "Add passenger Id : " + bookingCruisePassenger.IdPassenger.ToString() + " to booking Id : " + bookingCruisePassenger.IdBooking.ToString());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Add, CurrentUser, "Add passenger Id : " + bookingCruisePassenger.IdPassenger.ToString() + " to booking Id : " + bookingCruisePassenger.IdBooking.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region IsValidCruiseCode
        /// <summary>
        /// Retourne un indicateur d'existance du code de la croisière
        /// </summary>
        /// <param name="cruiseCode">Valeur du code de croisière à tester</param>
        /// <returns>Vrai si la croisière existe, faux sinon</returns>
        public bool IsValidCruiseCode(string cruiseCode)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(cruiseCode))
            {
                result = (from c in _shoreEntities.Cruise where c.Code.Equals(cruiseCode) select c.Id).Count();
            }
            return result > 0;
        }
        #endregion

        #region IsValidBookingCode
        /// <summary>
        /// Retourne un indicateur d'existance du code de la reservation
        /// </summary>
        /// <param name="bookingCode">Valeur du code de reservation à tester</param>
        /// <param name="cruiseCode">Code de croisière associé</param>
        /// <returns>Vrai si la reservation existe, faux sinon</returns>
        public bool IsValidBookingCode(string bookingCode, string cruiseCode)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(bookingCode))
            {
                if (string.IsNullOrEmpty(cruiseCode))
                {
                    result = (from b in _shoreEntities.Booking where b.Number.ToString().Equals(bookingCode) select b.Id).Count();
                }
                else
                {
                    result = (from bcp in _shoreEntities.BookingCruisePassenger where bcp.Booking.Number.ToString().Equals(bookingCode) && bcp.Cruise.Code.ToString().Equals(cruiseCode) select bcp.Booking.Id).Count();
                }
            }
            return result > 0;
        }
        #endregion

        #region Private

        #region AddDocumentCommon
        private void AddDocumentCommon(AddDocumentViewModel model, string fileName, string prefixFile, string currentUser)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                DateTime Now = DateTime.Now;
                try
                {
                    if (model.IdPassenger != 0)
                    {
                        string filePath = Path.Combine(AppSettings.FolderTemp, fileName);
                        Picture picture = new Picture(filePath);
                        picture.ConvertToJpeg();
                        List<string> files = picture.Treat();

                        if (files != null)
                        {
                            foreach (string file in files)
                            {
                                // Ajout du document
                                Document document = FillDocument(model.IdPassenger, Path.GetFileName(file), Now, currentUser, prefixFile);
                                _shoreEntities.Document.Add(document);
                                _shoreEntities.SaveChanges();
                                MoveDocumentFile(ref document, currentUser, prefixFile);
                                _shoreEntities.SaveChanges();

                                Passenger passenger = _shoreEntities.Passenger.Find(model.IdPassenger);
                                if (passenger != null)
                                {
                                    if (passenger.AutoAttachment == null)
                                    {
                                        passenger.AutoAttachment = false;
                                    }
                                    switch (document.Passenger.IdAdvice)
                                    {
                                        case Constants.NOT_APPLICABLE_NOT_APPLICABLE:
                                            document.Passenger.IdStatus = Constants.SHORE_STATUS_QM_RECEIVED;
                                            document.Passenger.Editor = currentUser;
                                            document.Passenger.ModificationDate = DateTime.Now;
                                            break;
                                        case Constants.ADVICE_WAITING_FOR_CLARIFICATION:
                                            document.Passenger.IdStatus = Constants.SHORE_STATUS_QM_NEW_DOCUMENTS;
                                            document.Passenger.Editor = currentUser;
                                            document.Passenger.ModificationDate = DateTime.Now;
                                            break;
                                    }
                                    _shoreEntities.SaveChanges();
                                }
                                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Document, LogManager.LogAction.Add, currentUser, "Add document Id : " + document.Id.ToString() + " to passenger Id : " + document.IdPassenger.ToString());
                            }
                            dbContextTransaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Document, LogManager.LogAction.Add, currentUser, "Add document : " + model.PassengerDocumentName + " to passenger Id : " + model.IdPassenger + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region FillDocument
        /// <summary>
        /// Rempli les données d'un document
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <param name="passengerDocumentName">Nom du fichier physique du document</param>
        /// <param name="now">Date courante</param>
        /// <param name="currentUser">Utilisateur courant</param>
        /// <returns></returns>
        private Document FillDocument(int idPassenger, string passengerDocumentName, DateTime now, string currentUser,string prefixFile)
        {
            // Ajout du document
            Document document = new Document
            {
                FileName = now.Year + "-" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0') + "-" +
                           now.Hour.ToString().PadLeft(2, '0') + "-" + now.Minute.ToString().PadLeft(2, '0') + "-" + now.Second.ToString().PadLeft(2, '0') + "_" + passengerDocumentName,
                Name = passengerDocumentName.Replace(currentUser + prefixFile, ""),
                IdPassenger = idPassenger,
                IdStatus = Constants.DOCUMENT_STATUS_NOT_SEEN,
                ReceiptDate = now,
                Creator = currentUser,
                CreationDate = now,
                Editor = currentUser,
                ModificationDate = now
            };
            return document;
        }
        #endregion

        #region MoveDocumentFile
        /// <summary>
        /// Déplace le fichier physique du document
        /// </summary>
        /// <param name="document">Document à ajouter</param>
        /// <param name="currentUser">Utilisateur courant</param>
        private void MoveDocumentFile(ref Document document, string currentUser, string prefixFile)
        {
            string fileName = FileManager.ReplaceCharFilename(currentUser + prefixFile + document.Name);
            if (!string.IsNullOrEmpty(fileName))
            {
                string destPath = GetDocumentPath(document);
                string newFileName = FileManager.FileMove(AppSettings.FolderTemp, fileName, destPath, document.FileName, true, true);
                document.FileName = newFileName;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}