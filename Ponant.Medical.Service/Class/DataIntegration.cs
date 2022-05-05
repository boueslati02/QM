using Ponant.Medical.Common;
using Ponant.Medical.Common.MailServer;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Auth;
using Ponant.Medical.Data.Shore;
using Ponant.Medical.Service.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ponant.Medical.Service.Class
{
    public class DataIntegration
    {
        #region Constants
        /// <summary>
        /// Encodage du fichier
        /// </summary>
        private const string FILE_ENCODING = "UTF-8";

        /// <summary>
        /// Statut d'annulation
        /// </summary>
        private const string STATUS_CANCEL = "CXL";
        #endregion

        #region Properties
        /// <summary>
        /// Nom du fichier à traiter
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Numéro du booking en cours de traitement
        /// </summary>
        public int BookingNumber { get; set; }

        /// <summary>
        /// Modification de la langue dans le fichier xml
        /// </summary>
        public bool IsLanguageUpdated { get; set; }

        /// <summary>
        /// Indique si le booking est a été intégré
        /// </summary>
        public bool IsIntegrated { get; set; }

        /// <summary>
        /// Indique si le fichier a été rejeté
        /// </summary>
        private bool isRejectedFile;

        /// <summary>
        /// Liste des erreurs de validation passager
        /// </summary>
        private List<ValidationEventArgs> rejectPassengers;

        /// <summary>
        /// Liste des erreurs de validation globale du fichier
        /// </summary>
        private List<ValidationEventArgs> rejectFileErrors;
        #endregion

        #region Public methods

        #region GetBookingNumber
        /// <summary>
        /// Obtient le numéro de booking à partir du nom du fichier
        /// </summary>
        /// <returns>Numéro de booking</returns>
        public int? GetBookingNumber()
        {
            string[] array = Path.GetFileName(FileName).Split('_');

            if (array.Length > 0)
            {
                string firstPart = array[0];
                string sBookingNumber = firstPart.Replace("BookingQM", "");

                if (int.TryParse(sBookingNumber, out int bookingNumber))
                {
                    return bookingNumber;
                }
            }

            return null;
        }
        #endregion

        #region ProcessIncomingFile
        /// <summary>
        /// Copie du fichier en local
        /// </summary>
        /// <param name="file">Fichier à intégrer</param>
        public bool ProcessIncomingFile()
        {
            bool isIntegrated = false;

            try
            {
                BookingNumber = GetBookingNumber().Value;

                // copie du fichier en local
                string localFile = Path.Combine(AppSettings.FolderShoreBooking, Path.GetFileName(FileName));
                File.Copy(FileName, localFile, true);

                // supprime le fichier distant
                File.Delete(FileName);
                isIntegrated = ProcessLocalFile(localFile);
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception);
            }

            return isIntegrated;
        }
        #endregion

        #endregion

        #region Private methods

        #region ProcessLocalFile
        /// <summary>
        /// Traitement du fichier local
        /// </summary>
        /// <param name="file">Fichier à intégrer</param>
        private bool ProcessLocalFile(string file)
        {
            bool isIntegrated = false;

            try
            {
                FileName = Path.GetFileName(file);
                isIntegrated = ProcessFile(file);
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception);
            }
            finally
            {
                if (!isIntegrated)
                {
                    // si traitement fichier à échoué => copie du fichier dans le répertoire d'erreur distant
                    string remoteFile = Path.Combine(AppSettings.FolderPonantBookingError, FileName);
                    File.Copy(file, remoteFile, true);
                }

                // suppression du fichier local
                File.Delete(file);
            }

            return isIntegrated;
        }
        #endregion

        #region ProcessFile
        /// <summary>
        /// Validation du fichier et intégration en base
        /// </summary>
        /// <param name="file">Fichier à traiter</param>
        /// <returns>Vrai si le fichier a été correctement intégré, faux sinon</returns>
        private bool ProcessFile(string file)
        {
            rejectFileErrors = new List<ValidationEventArgs>();

            // Intégration des fichier standard
            bool isIntegrated = ValidateFile(file, false);
            if (isIntegrated)
            {
                isIntegrated = AddToDatabase(file);
            }
            else
            {
                // Intégration des fichiers d'annulation
                isIntegrated = ValidateFile(file, true);
                if (isIntegrated)
                {
                    isIntegrated = AddToDatabase(file);
                }
            }

            // Log des erreur de validation
            if (!isIntegrated)
            {
                foreach (ValidationEventArgs args in rejectFileErrors)
                {
                    LogManager.InsertLog(args.Severity, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, args.Message, BookingNumber);
                }
            }

            rejectFileErrors.Clear();
            return isIntegrated;
        }
        #endregion

        #region ValidateFile
        /// <summary>
        /// Valide le fichier XML avec le schéma XSD
        /// </summary>
        /// <param name="file">Fichier à intégrer</param>
        /// <returns>Vrai si le fichier a été correctement intégré, faux sinon</returns>
        private bool ValidateFile(string file, bool isCancelFile)
        {
            bool isIntegrated = false;

            byte[] xsdValidatorFile = null;
            if (isCancelFile)
            {
                xsdValidatorFile = Resources.BookingQMCancel;
            }
            else
            {
                xsdValidatorFile = Resources.BookingQM;
            }

            if (xsdValidatorFile == null)
            {
                throw new FileNotFoundException("Invalid .xsd validator file");
            }

            try
            {
                // Create a schema validating XmlReader.
                XmlReaderSettings settings = new XmlReaderSettings();

                using (MemoryStream ms = new MemoryStream(xsdValidatorFile))
                {
                    using (XmlReader schemaReader = XmlReader.Create(ms))
                    {
                        settings.Schemas.Add(null, schemaReader);
                    }
                }

                settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
                settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationType = ValidationType.Schema;

                isRejectedFile = false;
                rejectPassengers = new List<ValidationEventArgs>();


                using (XmlReader reader = XmlReader.Create(file, settings))
                {
                    // The XmlDocument validates the XML document contained
                    // in the XmlReader as it is loaded into the DOM.
                    XmlDocument document = new XmlDocument();

                    // Valide le document complet
                    document.Load(reader);

                    // Valide chaque noeud passager
                    foreach (XmlNode node in document.SelectNodes(@"/Booking/ParticipantList/ParticipantData"))
                    {
                        document.Validate(ValidationEventHandler, node);
                    }
                }

                if (!isRejectedFile)
                {
                    IEqualityComparer<ValidationEventArgs> customComparer = new PropertyComparer<ValidationEventArgs>("Message");

                    foreach (ValidationEventArgs validation in rejectPassengers.Distinct(customComparer))
                    {
                        LogManager.InsertLog(validation.Severity, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, "Invalid participant data: " + validation.Message + " in file: " + FileName, BookingNumber);
                    }
                    rejectPassengers = null;
                    isIntegrated = true;
                }
            }
            catch (Exception exception)
            {
                LogManager.InsertLog(LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception);
            }

            return isIntegrated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (sender is XmlReader && !args.Message.Contains("ParticipantData"))
            {
                isRejectedFile = true;
                rejectFileErrors.Add(args);
            }
            else
            {
                rejectPassengers.Add(args);
            }
        }
        #endregion

        #region AddToDatabase
        /// <summary>
        /// Intègre les données du fichier en base de données
        /// </summary>
        /// <param name="file">Fichier à intégrer</param>
        /// <returns>Vrai si le fichier a été correctement intégré, faux sinon</returns>
        private bool AddToDatabase(string file)
        {
            bool isResult = IsIntegrated = false;

            Booking bookingRoot = GetBooking(file);

            if (bookingRoot != null)
            {
                // Traitement pour les booking annulé
                if (bookingRoot.BookingContext != null && bookingRoot.BookingContext.BookingStatusCode == STATUS_CANCEL)
                {
                    isResult = CancelBooking(bookingRoot);

                    if (isResult)
                    {
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Format("Annulation réussie du fichier {0}", Path.GetFileName(file)), BookingNumber);
                    }
                }
                else // Traitement pour les booking standard
                {
                    if (IsInsertable(bookingRoot))
                    {
                        UpdateReferencesTables(bookingRoot);
                        IsIntegrated = isResult = UpdateData(bookingRoot);

                        if (isResult)
                        {
                            LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Format("Intégration réussie du fichier {0}", Path.GetFileName(file)), BookingNumber);
                        }
                    }
                    else
                    {
                        isResult = true;
                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Format("Le fichier {0} ne satisfait aucun critère de croisière", Path.GetFileName(file)), BookingNumber);
                    }
                }
            }

            return isResult;
        }
        #endregion

        #region GetBooking
        /// <summary>
        /// Désérialisation de l'objet
        /// </summary>
        /// <param name="file">Fichier à désérialiser</param>
        /// <returns>Une instance de booking</returns>
        private static Booking GetBooking(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Booking));

            using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(FILE_ENCODING)))
            {
                return (Booking)serializer.Deserialize(sr);
            }
        }
        #endregion

        #region CancelBooking
        /// <summary>
        /// Annule une réservation
        /// </summary>
        /// <param name="bookingRoot">Instance du booking XML</param>
        /// <returns>Vrai si l'annulation a été prise en compte, faux sinon</returns>
        private bool CancelBooking(Booking bookingRoot)
        {
            bool isCanceled = false;

            using (ShoreEntities db = new ShoreEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Suppression
                        List<BookingCruisePassenger> bookingCruisePassengers = db.BookingCruisePassenger.Where(bcp => bcp.Booking.Number.Equals(bookingRoot.BookingContext.BookingNo)).ToList();

                        if (bookingCruisePassengers != null)
                        {
                            if (bookingCruisePassengers.Count > 0)
                            {
                                foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisePassengers)
                                {
                                    int nbBookingCruisePassenger = (from bcp in bookingCruisePassengers where bcp.IdPassenger.Equals(bookingCruisePassenger.IdPassenger) select bcp.Id).Distinct().Count(); // Nombre de croisière pour le passager
                                    int nbPassengerBooking = bookingCruisePassenger.Booking.BookingCruisePassenger.Distinct().Count(); // Nombre de passager dans le meme booking  

                                    // Déplacement des documents
                                    if (bookingCruisePassenger.Passenger.Document != null)
                                    {
                                        foreach (Document document in bookingCruisePassenger.Passenger.Document)
                                        {
                                            document.IdPassenger = Constants.NOT_APPLICABLE_NOT_APPLICABLE;
                                            document.ModificationDate = DateTime.Now;
                                            document.Editor = AppSettings.UserAccount;

                                            // Déplacement du fichier dans le bac à sable
                                            string sourcePath = Path.Combine(AppSettings.FolderPassenger, document.ReceiptDate.ToString("yyyy-MM"), bookingCruisePassenger.Passenger.Id.ToString(), document.FileName);
                                            if (File.Exists(sourcePath))
                                            {
                                                File.Move(sourcePath, Path.Combine(AppSettings.FolderSandBox, document.FileName));
                                            }
                                        }
                                    }

                                    if (nbBookingCruisePassenger <= 1 && bookingCruisePassenger.Passenger.BookingCruisePassenger != null && bookingCruisePassenger.Passenger.BookingCruisePassenger.Count <= 1)
                                    {
                                        db.Information.RemoveRange(bookingCruisePassenger.Passenger.Information);
                                        db.BookingActivity.RemoveRange(bookingCruisePassenger.Passenger.BookingActivity);
                                        db.Passenger.Remove(bookingCruisePassenger.Passenger);
                                    }

                                    if (nbPassengerBooking <= 1)
                                    {
                                        db.BookingActivity.RemoveRange(from ba in db.BookingActivity where ba.IdBooking.Equals(bookingCruisePassenger.Booking.Id) select ba);
                                        db.Booking.Remove(bookingCruisePassenger.Booking);
                                    }

                                    db.BookingCruisePassenger.Remove(bookingCruisePassenger);
                                    db.SaveChanges();
                                }

                                db.SaveChanges();
                                transaction.Commit();
                                isCanceled = true;
                            }
                            else
                            {
                                rejectFileErrors.Clear();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        if (exception.InnerException != null)
                        {
                            if (exception.InnerException.InnerException != null)
                            {
                                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Concat(exception.Message, " InnerException : ", exception.InnerException.InnerException.Message), BookingNumber);
                            }
                            else
                            {
                                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Concat(exception.Message, " InnerException : ", exception.InnerException.Message), BookingNumber);
                            }
                        }
                        else
                        {
                            LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception.Message, BookingNumber);
                        }
                    }
                }
            }

            return isCanceled;
        }
        #endregion

        #region UpdateReferencesTables
        /// <summary>
        /// MAJ des données de référence
        /// </summary>
        /// <param name="bookingRoot">Instance du booking XML</param>
        private void UpdateReferencesTables(Booking bookingRoot)
        {
            if (bookingRoot.BookingContext != null)
            {
                UpdateLov(Constants.LOV_LANGUAGE, bookingRoot.BookingContext.LanguageCode, bookingRoot.BookingContext.LanguageCode, true);
                UpdateLov(Constants.LOV_OFFICE, bookingRoot.BookingContext.OfficeName, bookingRoot.BookingContext.OfficeName, true);
            }

            if (bookingRoot.ParticipantList != null)
            {
                foreach (BookingParticipantData participant in bookingRoot.ParticipantList)
                {
                    if (!string.IsNullOrWhiteSpace(participant.Civility))
                    {
                        UpdateLov(Constants.LOV_CIVILITY, participant.Civility, participant.Civility, false);
                    }
                }
            }

            if (bookingRoot.CruiseBookings != null)
            {
                if (bookingRoot.CruiseBookings.CruiseSailing != null)
                {
                    foreach (BookingCruiseBookingsCruiseSailing cruise in bookingRoot.CruiseBookings.CruiseSailing)
                    {
                        UpdateLov(Constants.LOV_SHIP, cruise.ShipCode, cruise.ShipName, true);
                        UpdateLov(Constants.LOV_DESTINATION, cruise.DestinationCode, cruise.DestinationName, true);
                    }
                }

                if (bookingRoot.CruiseBookings.ActivityBookings != null)
                {
                    foreach (BookingCruiseBookingsActivityBooking activity in bookingRoot.CruiseBookings.ActivityBookings)
                    {
                        UpdateLov(Constants.LOV_ACTIVITY, activity.ActivityCode, activity.ActivityDescription, true);
                    }
                }
            }
        }
        #endregion

        #region UpdateData
        /// <summary>
        /// MAJ des données
        /// </summary>
        /// <param name="bookingRoot">Instance du booking XML</param>
        private bool UpdateData(Booking bookingRoot)
        {
            bool isIntegrated = false;
            bool isPassengerUpdatable = false;

            using (ShoreEntities db = new ShoreEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Agency agency = null;
                        Data.Shore.Booking booking = null;
  
                        if (bookingRoot.BookingContext != null)
                        {
                            #region Agency
                            if (bookingRoot.AgencyAddress != null)
                            {
                                agency = db.Agency
                                    .SingleOrDefault(a => a.Name.Equals(bookingRoot.AgencyAddress.AgencyName));

                                if (agency == null)
                                {
                                    agency = new Agency
                                    {
                                        Creator = AppSettings.UserAccount,
                                        CreationDate = DateTime.Now
                                    };
                                }

                                agency.Number = bookingRoot.AgencyAddress.AgencyId;
                                agency.Name = bookingRoot.AgencyAddress.AgencyName;
                                agency.LastName = bookingRoot.AgencyAddress.LastName;
                                agency.FirstName = bookingRoot.AgencyAddress.FirstName;
                                agency.Editor = AppSettings.UserAccount;
                                agency.ModificationDate = DateTime.Now;
#if DEV || INTEGRATION || RECETTE
                                if (!string.IsNullOrWhiteSpace(bookingRoot.AgencyAddress.Email))
                                {
                                    agency.Email = "x" + bookingRoot.AgencyAddress.Email;
                                }
#else
                                agency.Email = bookingRoot.AgencyAddress.Email;
#endif
                                if (agency.Id == 0)
                                {
                                    db.Agency.Add(agency);
                                }
                            }
                            else
                            {
                                agency = db.Agency
                                    .Single(a => a.Id.Equals(Constants.NOT_APPLICABLE_NOT_APPLICABLE));
                            }
                            #endregion

                            #region Booking

                            booking = db.Booking
                                .SingleOrDefault(b => b.Number.Equals(bookingRoot.BookingContext.BookingNo));
                            this.IsLanguageUpdated = false;

                            if (booking == null)
                            {
                                booking = new Data.Shore.Booking
                                {
                                    Number = bookingRoot.BookingContext.BookingNo,
                                    Creator = AppSettings.UserAccount,
                                    CreationDate = DateTime.Now
                                };
                            }
                            else
                            {
                                int newLanguageId = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_LANGUAGE) && l.Name.Equals(bookingRoot.BookingContext.LanguageCode)).Id;
                                this.IsLanguageUpdated = booking.IdLanguage != newLanguageId;
                            }

                            booking.IdLanguage = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_LANGUAGE) && l.Name.Equals(bookingRoot.BookingContext.LanguageCode)).Id;
                            booking.Agency = agency;
                            booking.IdOffice = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_OFFICE) && l.Name.Equals(bookingRoot.BookingContext.OfficeName)).Id;
                            booking.IsGroup = bookingRoot.BookingContext.IsGroup;
                            booking.GroupName = bookingRoot.BookingContext.GroupId;
                            booking.Editor = AppSettings.UserAccount;
                            booking.ModificationDate = DateTime.Now;

                            if (booking.Id == 0)
                            {
                                db.Booking.Add(booking);
                            }
                            #endregion
                        }

                        if (bookingRoot.CruiseBookings != null)
                        {
                            #region Cruise
                            Cruise cruise = null;
                            List<BookingCruisePassenger> bookingCruisePassengers = new List<BookingCruisePassenger>();
                            List<BookingActivity> bookingActivities = new List<BookingActivity>();

                            if (bookingRoot.CruiseBookings.CruiseSailing != null)
                            {
                                foreach (BookingCruiseBookingsCruiseSailing cruiseSailing in bookingRoot.CruiseBookings.CruiseSailing)
                                {
                                    if (IsInsertable(cruiseSailing, bookingRoot.CruiseBookings.ActivityBookings))
                                    {
                                        db.Cruise.Load();
                                        cruise = db.Cruise.Local
                                            .SingleOrDefault(c => c.Code.Equals(cruiseSailing.CruiseID));

                                        if (cruise == null)
                                        {
                                            cruise = new Cruise
                                            {
                                                Creator = AppSettings.UserAccount,
                                                CreationDate = DateTime.Now,
                                                IsExtract = false
                                            };
                                        }

                                        cruise.IdTypeCruise = string.IsNullOrEmpty(cruiseSailing.CruiseType) 
                                            ? Constants.NOT_APPLICABLE_NOT_APPLICABLE 
                                            : db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_CRUISE_TYPE) && l.Name.Equals(cruiseSailing.CruiseType)).Id;
                                        cruise.IdShip = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_SHIP) && l.Code.Equals(cruiseSailing.ShipCode)).Id;
                                        cruise.IdDestination = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_DESTINATION) && l.Code.Equals(cruiseSailing.DestinationCode)).Id;
                                        cruise.Code = cruiseSailing.CruiseID;
                                        cruise.SailingDate = cruiseSailing.SailingDate.Date;
                                        cruise.SailingLengthDays = cruiseSailing.SailingLengthDays;
                                        cruise.Editor = AppSettings.UserAccount;
                                        cruise.ModificationDate = DateTime.Now;

                                        using (AuthContext dbAuth = new AuthContext())
                                        {
                                            int? newIdShip = dbAuth.AspNetUsers
                                                .Where(u => u.AspNetUserShips.Select(us => us.IdShip).Contains(cruise.IdShip))
                                                .OrderBy(u => u.IdShip)
                                                .Select(u => u.IdShip)
                                                .FirstOrDefault();
                                                
                                            if(newIdShip.HasValue && newIdShip.Value != cruise.IdShip)
                                            {
                                                cruise.IdShip = newIdShip.Value;
                                            }
                                        }

                                        if (cruise.Id == 0)
                                        {
                                            db.Cruise.Add(cruise);
                                        }

                                        #region Passenger

                                        isPassengerUpdatable = !bookingRoot.BookingContext.IsGroup || (bookingRoot.BookingContext.IsGroup && !db.BookingCruisePassenger.Any(bp => bp.Booking.Number.Equals(bookingRoot.BookingContext.BookingNo) && !bp.Passenger.Creator.Equals(AppSettings.UserAccount)));

                                        if (isPassengerUpdatable && bookingRoot.ParticipantList != null)
                                        {
                                            //foreach (BookingParticipantData participant in bookingRoot.ParticipantList)
                                            foreach (int participantId in cruiseSailing.ParticipantIDs)
                                            {
                                                BookingParticipantData participant = bookingRoot.ParticipantList
                                                    .FirstOrDefault(pl => pl.PassengerNo == participantId.ToString());

                                                if (participant != null)
                                                {
                                                    int? passengerNumber = null;
                                                    DateTime? passengerBirthDate = null;

                                                    if (int.TryParse(participant.PassengerNo, out int tmpInt))
                                                    {
                                                        passengerNumber = tmpInt;
                                                    }

                                                    if (DateTime.TryParse(participant.DateOfBirth, out DateTime tmpDate))
                                                    {
                                                        passengerBirthDate = tmpDate;
                                                    }

                                                    db.Passenger.Load();
                                                    Passenger passenger = db.Passenger.Local
                                                        .FirstOrDefault(p => p.Number == passengerNumber && p.BookingCruisePassenger.Any(bcp => bcp.Booking.Number == booking.Number && bcp.Cruise.Code == cruiseSailing.CruiseID));

                                                    if (passenger == null)
                                                    {
                                                        passenger = new Passenger
                                                        {
                                                            Number = passengerNumber,
                                                            IsEmailUpdated = false,
                                                            SentCount = 0,
                                                            IsExtract = false,
                                                            IdAdvice = Constants.NOT_APPLICABLE_NOT_APPLICABLE,
                                                            IdStatus = Constants.NOT_APPLICABLE_NOT_APPLICABLE,
                                                            Creator = AppSettings.UserAccount,
                                                            CreationDate = DateTime.Now,
                                                        };
                                                    }
#if DEV || INTEGRATION || RECETTE
                                                    participant.Email = string.IsNullOrWhiteSpace(participant.Email)
                                                        ? participant.Email 
                                                        : "x" + participant.Email;
#endif
                                                    passenger.IsEmailValid = !string.IsNullOrWhiteSpace(participant.Email) && MailServer.IsValidEmail(participant.Email);
                                                    passenger.IdTitle = string.IsNullOrWhiteSpace(participant.Civility)
                                                        ? Constants.NOT_APPLICABLE_NOT_APPLICABLE
                                                        : db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_CIVILITY) && l.Name.Equals(participant.Civility)).Id;
                                                    passenger.Number = passengerNumber;
                                                    passenger.LastName = participant.LastName;
                                                    passenger.UsualName = participant.UsualName;
                                                    passenger.FirstName = participant.FirstName;
                                                    passenger.Email = participant.Email;
                                                    passenger.BirthDate = passengerBirthDate;
                                                    passenger.Editor = AppSettings.UserAccount;
                                                    passenger.ModificationDate = DateTime.Now;
                                                    passenger.IsDownloaded = false;

                                                    if (passenger.Id == 0)
                                                    {
                                                        db.Passenger.Add(passenger);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region BookingCruisePassenger
                                        if (isPassengerUpdatable && cruiseSailing.ParticipantIDs != null)
                                        {
                                            foreach (int idParticipant in cruiseSailing.ParticipantIDs)
                                            {
                                                db.Passenger.Load();
                                                Passenger passenger = db.Passenger.Local
                                                    .Where(p => p.Number.Equals(idParticipant))
                                                    .OrderByDescending(p => p.CreationDate)
                                                    .AsEnumerable()
                                                    .FirstOrDefault();

                                                BookingCruisePassenger bookingCruisePassenger = null;

                                                if (booking.Id != 0 && cruise.Id != 0 && passenger.Id != 0)
                                                {
                                                    bookingCruisePassenger = db.BookingCruisePassenger
                                                        .SingleOrDefault(bcp => bcp.IdBooking.Equals(booking.Id) && bcp.IdCruise.Equals(cruise.Id) && bcp.IdPassenger.Equals(passenger.Id));

                                                    if (bookingCruisePassenger == null) // Gère le cas ou ce n'est pas la dernière occurence du passager qui doit être utilisé
                                                    {
                                                        List<int> passengersIdList = db.Passenger.Local
                                                            .Where(p => p.Number.Equals(idParticipant))
                                                            .OrderByDescending(p => p.CreationDate)
                                                            .Select(p => p.Id)
                                                            .ToList();
                                                        if (passengersIdList != null && passengersIdList.Count > 1)
                                                        {
                                                            bookingCruisePassenger = db.BookingCruisePassenger
                                                                .Where(bcp => bcp.IdBooking.Equals(booking.Id) && bcp.IdCruise.Equals(cruise.Id) && passengersIdList.Contains(bcp.IdPassenger))
                                                                .FirstOrDefault();
                                                            if (bookingCruisePassenger != null)
                                                            {
                                                                passenger = bookingCruisePassenger.Passenger;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (bookingCruisePassenger == null)
                                                {
                                                    bookingCruisePassenger = new BookingCruisePassenger
                                                    {
                                                        Booking = booking,
                                                        Cruise = cruise,
                                                        Passenger = passenger,
                                                        IsEnabled = true,
                                                        Creator = AppSettings.UserAccount,
                                                        CreationDate = DateTime.Now
                                                    };
                                                }

                                                bookingCruisePassenger.CabinNumber = cruiseSailing.CabinNo;
                                                bookingCruisePassenger.Editor = AppSettings.UserAccount;
                                                bookingCruisePassenger.ModificationDate = DateTime.Now;

                                                if (bookingCruisePassenger.Id == 0)
                                                {
                                                    db.BookingCruisePassenger.Add(bookingCruisePassenger);
                                                }

                                                bookingCruisePassengers.Add(bookingCruisePassenger);
                                            }
                                        }
                                        #endregion

                                        #region Activity
                                        db.BookingActivity.RemoveRange(booking.BookingActivity);

                                        if (isPassengerUpdatable && bookingRoot.CruiseBookings.ActivityBookings != null)
                                        {
                                            db.Passenger.Load();

                                            foreach (BookingCruiseBookingsActivityBooking activity in bookingRoot.CruiseBookings.ActivityBookings)
                                            {
                                                if (activity.ParticipantIDs != null)
                                                {
                                                    foreach (int idParticipant in activity.ParticipantIDs)
                                                    {
                                                        db.Passenger.Load();
                                                        Passenger passenger = db.Passenger.Local
                                                            .Where(p => p.Number.Equals(idParticipant)
                                                            && (p.BookingCruisePassenger.Any(bcp => bcp.IdCruise == cruise.Id) || cruise.Id == 0))
                                                            .OrderByDescending(p => p.CreationDate)
                                                            .AsEnumerable()
                                                            .FirstOrDefault();

                                                        if(passenger != null)
                                                        {
                                                            int idActivity = db.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_ACTIVITY) && l.Code.Equals(activity.ActivityCode)).Id;
                                                            bool activityAlreadyInserted = bookingActivities
                                                                .Any(ba => ba.IdActivity == idActivity
                                                                && ba.Booking.Number == booking.Number
                                                                && ba.Passenger.Number == passenger.Number);

                                                            if (!activityAlreadyInserted) //Verifie si l'activite a déjà ete inserer pour eviter les insertion en doublon
                                                            {
                                                                BookingActivity bookingActivity = new BookingActivity
                                                                {
                                                                    Booking = booking,
                                                                    Passenger = passenger,
                                                                    IdActivity = idActivity,
                                                                    Creator = AppSettings.UserAccount,
                                                                    CreationDate = DateTime.Now,
                                                                    Editor = AppSettings.UserAccount,
                                                                    ModificationDate = DateTime.Now
                                                                };

                                                                bookingActivities.Add(bookingActivity);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, string.Format("La croisière {0} ne satisfait aucun critère de croisière", cruiseSailing.CruiseID), BookingNumber);
                                    }
                                }
                            }

                            if(bookingActivities != null && bookingActivities.Any())
                            {
                                booking.BookingActivity = bookingActivities;
                            }

                            // Suppression des tous les lignes présentes en base mais plus dans le fichier XML
                            db.BookingCruisePassenger.Load();
                            List<BookingCruisePassenger> bookingCruisePassengersToDelete = db.BookingCruisePassenger.Local
                                .Where(bcp => bookingCruisePassengers.Any(c => bcp.Booking.Number.Equals(c.Booking.Number)) && (!bookingCruisePassengers.Any(c => bcp.Cruise.Code.Equals(c.Cruise.Code)) || !bookingCruisePassengers.Any(c => bcp.Passenger.Number.Equals(c.Passenger.Number))))
                                .ToList();
                            db.BookingCruisePassenger.RemoveRange(bookingCruisePassengersToDelete);

                            #endregion
                        }

                        // Enregistrement en base
                        db.SaveChanges();
                        transaction.Commit();
                        isIntegrated = true;
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception.Message, BookingNumber);

                        if (exception.InnerException != null)
                        {
                            LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.FluxBooking, LogManager.LogAction.Integration, AppSettings.UserAccount, exception.InnerException.Message, BookingNumber);
                        }
                    }
                }
            }

            return isIntegrated;
        }
        #endregion

        #region IsInsertable
        /// <summary>
        /// Définit si des critères de croisières permettent d'intégrer le fichier
        /// </summary>
        /// <param name="booking">Instance du booking</param>
        /// <returns>Vrai si le fichier est intégrable, faux sinon</returns>
        private bool IsInsertable(Booking booking)
        {
            bool isInsertable = false;

            using (ShoreEntities db = new ShoreEntities())
            {
                foreach (BookingCruiseBookingsCruiseSailing cruise in booking.CruiseBookings.CruiseSailing)
                {
                    isInsertable = IsInsertable(cruise, booking.CruiseBookings.ActivityBookings);

                    if (isInsertable)
                    {
                        break;
                    }
                }
            }

            return isInsertable;
        }

        /// <summary>
        /// Définit si des critères de croisières permettent d'intégrer la croisière courante
        /// </summary>
        /// <param name="cruise">Instance de la croisière</param>
        /// <param name="activities">Liste des activités</param>
        /// <returns>Vrai si la croisière est intégrable, faux sinon</returns>
        private bool IsInsertable(BookingCruiseBookingsCruiseSailing cruise, BookingCruiseBookingsActivityBooking[] activities)
        {
            bool isInsertable = false;

            if (cruise == null)
            {
                return false;
            }

            using (ShoreEntities db = new ShoreEntities())
            {
                IQueryable<CruiseCriterion> criterion = db.CruiseCriterion;

                criterion = criterion.Where(c => (c.LovCruiseType.Name.Equals(cruise.CruiseType) || c.IdCruiseType == 0)
                    && (c.CruiseCriterionDestination.Any(d => d.LovDestination.Code.Equals(cruise.DestinationCode)) || !c.CruiseCriterionDestination.Any())
                    && (c.CruiseCriterionShip.Any(s => s.LovShip.Code.Equals(cruise.ShipCode)) || !c.CruiseCriterionShip.Any())
                    && (cruise.SailingLengthDays > c.Length || !c.Length.HasValue) &&
                    (c.Cruise.Contains(cruise.CruiseID) || string.IsNullOrEmpty(c.Cruise)));

                if (activities != null)
                {
                    foreach (BookingCruiseBookingsActivityBooking activity in activities)
                    {
                        IQueryable<CruiseCriterion> criterionActivity = criterion
                            .Where(c => c.Activity.Contains(activity.ActivityCode) || string.IsNullOrEmpty(c.Activity));

                        isInsertable = criterionActivity.Any();
                        if (isInsertable)
                        {
                            break;
                        }
                    }
                }

                criterion = criterion
                    .Where(c => string.IsNullOrEmpty(c.Activity))
                    .AsQueryable();

                bool result = isInsertable || criterion.Any();
                return result;
            }
        }
        #endregion

        #region UpdateLov
        /// <summary>
        /// Met à jour les liste de valeurs si besoin
        /// </summary>
        /// <param name="idLovType">Identifiant du type de liste</param>
        /// <param name="code">Code de la valeur</param>
        /// <param name="name">Nom de la valeur</param>
        /// <param name="isEnabled">La valeur est active</param>
        private void UpdateLov(int idLovType, string code, string name, bool isEnabled)
        {
            using (ShoreEntities db = new ShoreEntities())
            {
                if (!db.Lov.Any(l => l.IdLovType.Equals(idLovType) && l.Code.Equals(code)))
                {
                    db.Lov.Add(new Lov
                    {
                        IdLovType = idLovType,
                        Code = code,
                        Name = name,
                        IsEnabled = isEnabled,
                        Creator = AppSettings.UserAccount,
                        CreationDate = DateTime.Now,
                        Editor = AppSettings.UserAccount,
                        ModificationDate = DateTime.Now
                    });

                    db.SaveChanges();
                }
            }
        }
        #endregion

        #endregion
    }
}