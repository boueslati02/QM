namespace Ponant.Medical.Shore.Models
{
    using CsvHelper;
    using CsvHelper.Configuration;
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    #region Customize Object

    #region PassengersGroup
    /// <summary>
    /// Representation de l'object complexe PassengersGroup
    /// </summary>
    public class PassengersGroup
    {
        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public int NbForGroup { get; set; }

        public int NbIdentify { get; set; }

        public int NbQMReceive { get; set; }

        public int NbQMValidate { get; set; }

        public int NbQMWaiting { get; set; }

        public int NbQMUnfavorableAdvice { get; set; }

        public int BookingNumber { get; set; }

        public string AgencyName { get; set; }

        public int IdAgency { get; set; }

        #endregion
    }
    #endregion

    #endregion

    #region Modèles des vues

    #region PassengerViewModel
    /// <summary>
    /// Représentation pour la liste des passagers
    /// </summary>
    public class PassengerViewModel
    {
        /// <summary>
        /// Identifiant de la croisière
        /// </summary>
        public int IdCruise { get; set; }

        /// <summary>
        /// Code de la croisière
        /// </summary>
        public string CruiseCode { get; set; }

        /// <summary>
        /// Nombre de passagers attendus
        /// </summary>
        public int NbPassenger { get; set; }

        /// <summary>
        /// Nombre de QM reçus à terre
        /// </summary>
        public int NbQMShore { get; set; }

        /// <summary>
        /// Nombre de QM téléchargés à bord
        /// </summary>
        public int NbQMBoard { get; set; }

        /// <summary>
        /// Nom du bateau qui a extrait les QM
        /// </summary>
        public string ShipName { get; set; }

        /// <summary>
        /// Nombre de QM traités sur terre et à bord
        /// </summary>
        public int NbQMTreat { get; set; }

        /// <summary>
        /// Nombre de QM téléchargés avant croisiére
        /// </summary>
        public int NbQMDowloadCruise { get; set; }

        /// <summary>
        /// Nombre de passagers individuel
        /// </summary>
        public int NbIndiv { get; set; }

        /// <summary>
        /// Nombre de QM reçu
        /// </summary>
        public int NbIndivReceive { get; set; }

        /// <summary>
        /// Nombre de QM validés
        /// </summary>
        public int NbIndivValidate { get; set; }

        /// <summary>
        /// Nombre de QM en attente de précision
        /// </summary>
        public int NbIndivWaiting { get; set; }

        /// <summary>
        /// Nombre d'avis non favorable
        /// </summary>
        public int NbIndivUnfavorableAdvice { get; set; }

        /// <summary>
        /// Liste des groupes de passagers
        /// </summary>
        public List<PassengersGroup> ListOfGroups { get; set; }
    }
    #endregion

    #region PassengerModel
    public class PassengerModel
    {
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

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Title")]
        public int Civility { get; set; }

        [Required]
        [Display(Name = "N° booking")]
        public int BookingNumber { get; set; }
    }
    #endregion

    #region CreateAgencyPassengerViewModel
    public class CreateAgencyPassengerViewModel : PassengerModel
    {
        [Required]
        [Display(Name = "Cruise")]
        public int CruiseId { get; set; }
    }
    #endregion

    #region EditPassenger

    #region EditPassengerModel
    public class EditPassengerModel : PassengerModel
    {
        public int PassengerId { get; set; }

        public int CruiseId { get; set; }

        public int BookingId { get; set; }
    }
    #endregion

    #region EditPassengerViewModel
    public class EditPassengerViewModel
    {
        public int PassengerId { get; set; }

        public int CruiseId { get; set; }

        public int BookingId { get; set; }

        [Required]
        [Display(Name = "LastName")]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
    }
    #endregion

    #region EditAgencyPassengerViewModel
    public class EditAgencyPassengerViewModel : EditPassengerModel
    {
        [Display(Name = "Re-Send Survey")]
        public bool ReSendSurvey { get; set; }
    }
    #endregion

    #endregion

    #region PassengerImport
    public class PassengerImport
    {
        public int BookingNumber { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public string UsualName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }
    #endregion

    #endregion

    #region Gestion des passagers
    /// <summary>
    /// Classe de gestion des passagers
    /// </summary>
    public class PassengerClass : SharedPassengerClass
    {
        #region Properties & Constructors

        public PassengerClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region AddPassenger
        /// <summary>
        /// Ajoute un nouveau passager
        /// </summary>
        /// <param name="model">Model d'ajout d'un passager</param>
        public int? AddPassenger(CreateAgencyPassengerViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                BookingCruisePassenger bookingCruisePassenger = new BookingCruisePassenger();
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
                        IdStatus = Constants.SHORE_STATUS_QM_NOT_SENT,
                        IdTitle = model.Civility,
                        Email = model.Email,
                        AutoAttachment = null,
                        Number = null,
                        IsDownloaded = false
                    };

                    _shoreEntities.Passenger.Add(passenger);
                    _shoreEntities.SaveChanges();
                    int idPassenger = _shoreEntities.Passenger.Local.Max(p => p.Id);

                    bookingCruisePassenger = new BookingCruisePassenger
                    {
                        CabinNumber = null,
                        IsEnabled = true,
                        Creator = CurrentUser,
                        CreationDate = Now,
                        Editor = CurrentUser,
                        ModificationDate = Now,
                        IdPassenger = idPassenger,
                        IdBooking = _shoreEntities.Booking.Where(b => b.Number == model.BookingNumber).First().Id,
                        IdCruise = model.CruiseId,
                    };
                    _shoreEntities.BookingCruisePassenger.Add(bookingCruisePassenger);
                    _shoreEntities.SaveChanges();

                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Add, CurrentUser, "Add passenger Id : " + bookingCruisePassenger.IdPassenger.ToString() + " to booking Id : " + bookingCruisePassenger.IdBooking.ToString());
                    return idPassenger;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Add, CurrentUser, "Add passenger Id : " + bookingCruisePassenger.IdPassenger.ToString() + " to booking Id : " + bookingCruisePassenger.IdBooking.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    return null;
                }

            }
        }
        #endregion

        #region GetCruiseData
        /// <summary>
        /// Retourne les données de la croisiére pour affichage
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <returns></returns>
        public PassengerViewModel GetCruiseData(int id)
        {
            ApplicationDbContext dbAuth = DependencyResolver.Current.GetService<ApplicationDbContext>();
            UserClass _userClass = new UserClass(dbAuth);

            PassengerViewModel model = new PassengerViewModel();

            // Liste de tous les passagers de la croisière
            List<vPassengerShore> passengerShore = (from ps in _shoreEntities.vPassengerShore where ps.IdCruise.Equals(id) orderby ps.GroupName, ps.Id select ps).Distinct().ToList();

            Cruise cruise = _shoreEntities.Cruise.Find(id);
            model.IdCruise = cruise.Id;
            model.CruiseCode = cruise.Code;
            model.NbPassenger = (from ps in passengerShore where ps.IsEnabled select ps.Id).Distinct().Count();
            model.NbQMShore = (from ps in passengerShore
                               where (ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_RECEIVED) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_CLOSED) ||
                                ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_NEW_DOCUMENTS) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_IN_PROGRESS) ||
                                ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_INCOMPLETE))
                               select ps.Id).Distinct().Count();
            model.NbQMBoard = (from ps in passengerShore where ps.IsExtract && ps.IsEnabled select ps.Id).Distinct().Count();
            model.ShipName = cruise.LovShip.Name;
            model.NbQMTreat = (from ps in passengerShore
                               where (ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS) ||
                                ps.IdAdvice.Equals(Constants.ADVICE_UNFAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_WAITING_FOR_CLARIFICATION))
                               select ps.Id).Distinct().Count();
            model.NbQMDowloadCruise = (from ps in passengerShore where ps.IsEnabled && ps.IsDownloaded select ps.Id).Distinct().Count();

            model.NbIndiv = (from ps in passengerShore where !ps.IsGroup && ps.IsEnabled select ps.Id).Distinct().Count();
            model.NbIndivReceive = (from ps in passengerShore
                                    where !ps.IsGroup && ps.IsEnabled && (ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_RECEIVED) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_CLOSED) ||
                                    ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_NEW_DOCUMENTS) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_IN_PROGRESS))
                                    select ps.Id).Distinct().Count();
            model.NbIndivValidate = (from ps in passengerShore where !ps.IsGroup && ps.IsEnabled && (ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS)) select ps.Id).Distinct().Count();
            model.NbIndivWaiting = (from ps in passengerShore where !ps.IsGroup && ps.IsEnabled && ps.IdAdvice.Equals(Constants.ADVICE_WAITING_FOR_CLARIFICATION) select ps.Id).Distinct().Count();
            model.NbIndivUnfavorableAdvice = (from ps in passengerShore where !ps.IsGroup && ps.IsEnabled && ps.IdAdvice.Equals(Constants.ADVICE_UNFAVORABLE_OPINION) select ps.Id).Distinct().Count();

            model.ListOfGroups = new List<PassengersGroup>();

            List<int> idsBookingGroups = (from ps in passengerShore where ps.IsGroup orderby ps.IdBooking select ps.IdBooking).Distinct().ToList();
            foreach (int idBookingGroups in idsBookingGroups)
            {
                Booking booking = _shoreEntities.Booking.Find(idBookingGroups);

                IQueryable<Data.Shore.vAgencyAccessRight> agencyAccessRightRequest = _shoreEntities.vAgencyAccessRight
                                                                            .Where(aar => aar.BookingNumber.Equals(booking.Number));

                PassengersGroup passengerGroup = new PassengersGroup
                {
                    Id = booking.Id,
                    Name = booking.GroupName,
                    NbForGroup = (from ps in passengerShore where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) select ps.Id).Distinct().Count(),
                    NbIdentify = (from ps in passengerShore where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) select ps.Id).Distinct().Count(),
                    NbQMReceive = (from ps in passengerShore
                                   where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) && (ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_RECEIVED) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_CLOSED) ||
                                   ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_NEW_DOCUMENTS) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_IN_PROGRESS))
                                   select ps.Id).Distinct().Count(),
                    NbQMValidate = (from ps in passengerShore where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) && (ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS)) select ps.Id).Distinct().Count(),
                    NbQMWaiting = (from ps in passengerShore where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) && ps.IdAdvice.Equals(Constants.ADVICE_WAITING_FOR_CLARIFICATION) select ps.Id).Distinct().Count(),
                    NbQMUnfavorableAdvice = (from ps in passengerShore where ps.IsGroup && ps.IsEnabled && ps.IdBooking.Equals(booking.Id) && ps.IdAdvice.Equals(Constants.ADVICE_UNFAVORABLE_OPINION) select ps.Id).Distinct().Count(),
                    BookingNumber = booking.Number
                };

                if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
                {
                    string userId = _userClass.GetUserId(HttpContext.Current.User.Identity.Name);
                    int agencyId = _userClass.GetAgencyId(userId);

                    Data.Shore.vAgencyAccessRight agencyAccessRight = agencyAccessRightRequest
                                                                            .Where(aar => aar.IdAgency.Equals(agencyId))
                                                                            .FirstOrDefault();
                    if (agencyAccessRight != null)
                    {
                        passengerGroup.AgencyName = agencyAccessRight.AgencyName;
                        passengerGroup.IdAgency = agencyAccessRight.IdAgency;
                        model.ListOfGroups.Add(passengerGroup);
                    }
                }
                else
                {
                    Data.Shore.vAgencyAccessRight agencyAccessRight = agencyAccessRightRequest.FirstOrDefault();
                    passengerGroup.AgencyName = agencyAccessRight != null ? agencyAccessRight.AgencyName : AppSettings.AgencyName;
                    passengerGroup.IdAgency = agencyAccessRight != null ? agencyAccessRight.IdAgency : AppSettings.IdAgency;
                    model.ListOfGroups.Add(passengerGroup);
                }
            }
            return model;
        }
        #endregion

        #region GetPassenger
        /// <summary>
        /// Retourne un passagers pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du passager/param>
        /// <returns>Un passager</returns>
        public EditPassengerViewModel GetPassenger(int id)
        {
            EditPassengerViewModel model = new EditPassengerViewModel();

            Passenger passager = _shoreEntities.Passenger.Find(id);
            if (passager != null)
            {
                model.PassengerId = passager.Id;
                model.Email = passager.Email;
                model.FirstName = passager.FirstName;
                model.LastName = passager.LastName;
            }

            return model;
        }
        #endregion

        #region GetTemplateFile
        public byte[] GetTemplateFile(int idBooking)
        {
            byte[] file = null;

            try
            {
                List<PassengerImport> passengerImports = new List<PassengerImport>
            {
                new PassengerImport
                {
                    BookingNumber = _shoreEntities.Booking.Find(idBooking).Number
                }
            };

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
                        csvConfiguration.SanitizeForInjection = false;
                        csvConfiguration.Delimiter = ";";

                        using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration))
                        {
                            csvWriter.WriteRecords(passengerImports);
                        }

                        file = memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Get, HttpContext.Current.User.Identity.Name, "Get template file idBooking " + idBooking + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }

            return file;
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un passager
        /// </summary>
        /// <param name="">Passager à modifier</param>
        public void Edit(EditPassengerViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            int? bookingNumber = (from bcp in _shoreEntities.BookingCruisePassenger where bcp.IdPassenger.Equals(model.PassengerId) && bcp.IdCruise.Equals(model.CruiseId) select bcp.Booking.Number).FirstOrDefault();
            try
            {
                Passenger passenger = _shoreEntities.Passenger.Find(model.PassengerId);
                passenger.Email = model.Email;
                passenger.IsEmailUpdated = true;
                passenger.Editor = CurrentUser;
                passenger.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, CurrentUser, "Edit passenger Id : " + model.PassengerId.ToString(), bookingNumber);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Edit, CurrentUser, "Edit passenger Id : " + model.PassengerId.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")", bookingNumber);
                throw;
            }
        }
        #endregion

        #region GetAgencyPassenger
        /// <summary>
        /// Retourne un passager associé à l'agence pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du passager/param>
        /// <returns>Un passager</returns>
        public EditAgencyPassengerViewModel GetAgencyPassenger(int id, int idBooking)
        {
            EditAgencyPassengerViewModel model = new EditAgencyPassengerViewModel();

            Passenger passager = _shoreEntities.Passenger.Find(id);
            if (passager != null)
            {
                model.PassengerId = passager.Id;
                model.BookingNumber = _shoreEntities.Booking.Where(b => b.Id == idBooking).Single().Number;
                model.Civility = passager.IdTitle;
                model.Email = passager.Email;
                model.FirstName = passager.FirstName;
                model.UsualName = passager.UsualName;
                model.LastName = passager.LastName;
            }

            return model;
        }

        /// <summary>
        /// Ajoute un nouveau passager associé à une agence
        /// </summary>
        /// <param name="model">Model d'ajout d'un passager</param>
        public int? EditAgencyPassenger(EditAgencyPassengerViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            try
            {
                Passenger passenger = _shoreEntities.Passenger.Find(model.PassengerId);
                passenger.IdTitle = model.Civility;
                passenger.Number = null;
                passenger.LastName = model.LastName;
                passenger.UsualName = model.UsualName;
                passenger.FirstName = model.FirstName;
                passenger.Email = model.Email;
                passenger.Editor = CurrentUser;
                passenger.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, CurrentUser, "Edit passenger Id : " + model.PassengerId.ToString(), model.BookingNumber);
                return model.PassengerId;
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Edit, CurrentUser, "Edit passenger Id : " + model.PassengerId.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")", model.BookingNumber);
                throw;
            }
        }
        #endregion

        #region ImportPassenger
        public List<int> ImportPassenger(Stream stream, int idBooking, int idCruise)
        {
            List<int> idPassengers = new List<int>();
            try
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1")))
                {
                    CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
                    csvConfiguration.SanitizeForInjection = false;
                    csvConfiguration.Delimiter = ";";
                    csvConfiguration.HasHeaderRecord = true;
                    csvConfiguration.Encoding = Encoding.GetEncoding("iso-8859-1");

                    using (CsvReader csvReader = new CsvReader(reader, csvConfiguration))
                    {
                        IEnumerable<PassengerImport> passengerImports = csvReader.GetRecords<PassengerImport>().ToList();
                        Booking booking = _shoreEntities.Booking.Find(idBooking);

                        foreach (PassengerImport passengerImport in passengerImports)
                        {
                            if (passengerImport.BookingNumber == booking.Number)
                            {
                                int? idPassager = AddPassenger(new CreateAgencyPassengerViewModel
                                {
                                    BookingNumber = booking.Number,
                                    Civility = _shoreEntities.Lov.Single(l => l.IdLovType.Equals(Constants.LOV_CIVILITY) && l.Name.Equals(passengerImport.Title)).Id,
                                    CruiseId = idCruise,
                                    LastName = passengerImport.LastName,
                                    UsualName = passengerImport.UsualName,
                                    FirstName = passengerImport.FirstName,
                                    Email = passengerImport.Email
                                });

                                if (idPassager.HasValue)
                                {
                                    idPassengers.Add(idPassager.Value);
                                }
                            }
                        }
                    }
                }
                return idPassengers;
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Import passenger idBooking " + idBooking + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region SendIndividualRelaunch
        /// <summary>
        /// Envoi des email de relance des passagers individuel
        /// </summary>
        /// <param name="idRelaunchPassengers">Liste des identifiants des passagers concerné</param>
        /// <param name="cruiseId">Identifiant de la croisière</param>
        /// <returns>Dictionnaire des compteur de résultat</returns>
        public async Task<CounterRelaunchResult> SendIndividualRelaunch(List<int> idRelaunchPassengers, int cruiseId, bool statusNeedToBeUpdate)
        {
            List<Passenger> passengers = (from p in _shoreEntities.Passenger where idRelaunchPassengers.Contains(p.Id) select p)
                    .Distinct()
                    .Include("LovTitle")
                    .Include("BookingCruisePassenger")
                    .Include("BookingCruisePassenger.Cruise")
                    .Include("BookingCruisePassenger.Cruise.LovTypeCruise")
                    .Include("BookingCruisePassenger.Cruise.LovDestination")
                    .Include("BookingCruisePassenger.Cruise.LovShip")
                    .ToList();

            return await SendRelaunchByPassenger(passengers, cruiseId, null, false, statusNeedToBeUpdate);
        }
        #endregion

        #region SendGroupRelaunch
        /// <summary>
        /// Envoi des email de relance des passagers de groupe
        /// </summary>
        /// <param name="idBooking">Identifiant du booking</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        public async Task<bool> SendGroupRelaunch(int idBooking, int idCruise, bool statusNeedToBeUpdate = false)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                string fileMergeName = null;
                BookingCruisePassenger bookingCruisePassenger = _shoreEntities.BookingCruisePassenger.First(bcp => bcp.IdBooking.Equals(idBooking) && bcp.IdCruise.Equals(idCruise));

                try
                {
                    Cruise cruise = _shoreEntities.Cruise.Find(idCruise);
                    CruiseCriteria cruiseCriteria = new CruiseCriteria(_shoreEntities);
                    CruiseCriterion cruisecriterion = cruiseCriteria.GetCriteria(cruise.BookingCruisePassenger); // Récupération du critére le plus pertinent
                    cruisecriterion = _shoreEntities.CruiseCriterion.Find(cruisecriterion.Id);

                    if (cruisecriterion == null) // Absence de critère conforme existant
                    {
                        throw new Exception("No criteria correspond to this cruise");
                    }
                    if (cruisecriterion.Survey.Language == null || cruisecriterion.Survey.Language.Count == 0) // Absence de langage pour le questionnaire associé à ce critére
                    {
                        throw new Exception("No language available for the associated survey");
                    }

                    if (!string.IsNullOrEmpty(AppSettings.AddressGroupTo))
                    {

                        Language language = (from l in cruisecriterion.Survey.Language
                                             where !string.IsNullOrEmpty(l.GroupSurveyMail) &&
                                            (l.IdLanguage.Equals(bookingCruisePassenger.Booking.IdLanguage) || l.IsDefault)
                                             orderby l.IsDefault
                                             select l).FirstOrDefault(); // Récupération du langage 

                        // Récupération du chemin du modèle de message
                        string mailFilename = null;
                        string mailFilePath = null;
                        if (language != null)
                        {
                            mailFilename = language.GroupSurveyMail;
                            if (mailFilename != null)
                            {
                                mailFilePath = FileManager.FileGetPath(AppSettings.FolderMailGroup, language.Id.ToString() + mailFilename, false);
                            }
                        }

                        if (string.IsNullOrEmpty(mailFilePath))
                        {
                            throw new FileNotFoundException("Any group email template found");
                        }

                        Storage.Message message = new Storage.Message(mailFilePath);
                        if (message != null)
                        {
                            // Liste des booking passengers non clos
                            List<BookingCruisePassenger> listBookingPassengerSend = (from bcp in _shoreEntities.BookingCruisePassenger where bcp.IdBooking == idBooking && (bcp.Passenger.IdStatus == Constants.SHORE_STATUS_QM_NOT_SENT || bcp.Passenger.IdStatus == Constants.SHORE_STATUS_QM_SENT || bcp.Passenger.IdStatus == Constants.SHORE_STATUS_QM_INCOMPLETE) select bcp).Distinct().ToList();
                            // Liste des booking passengers clos
                            List<BookingCruisePassenger> listBookingPassengerClosed = (from bcp in _shoreEntities.BookingCruisePassenger where bcp.IdBooking == idBooking && bcp.Passenger.IdStatus != Constants.SHORE_STATUS_QM_NOT_SENT && bcp.Passenger.IdStatus != Constants.SHORE_STATUS_QM_SENT && bcp.Passenger.IdStatus != Constants.SHORE_STATUS_QM_INCOMPLETE select bcp).Distinct().ToList();

                            // Envoi du message
                            using (message)
                            {
                                string bodyValue = message.BodyHtml;

                                // Remplacement des balises [TABLE]
                                Regex regex = new Regex(Regex.Escape(AppSettings.TagTable), RegexOptions.IgnoreCase);
                                bodyValue = bodyValue.Replace(AppSettings.TagTableNoDocument, (listBookingPassengerSend.Count > 0
                                    ? MailServer.BuildTableForRelaunch(listBookingPassengerSend)
                                    : "No passenger for this section")); // Replacement 1ere table par la liste des passager incomplet

                                bodyValue = bodyValue.Replace(AppSettings.TagTableDocument, (listBookingPassengerClosed.Count > 0
                                    ? MailServer.BuildTableForRelaunch(listBookingPassengerClosed)
                                    : "No passenger for this section")); // Replacement 2eme table par la liste des passager complet

                                // Ajout de la pièce jointe
                                List<string> attachementList = new List<string>();
                                string filePathSurvey = FileManager.FileGetPath(AppSettings.FolderSurveyGroup, language.Id.ToString() + language.GroupSurveyFileName, true);
                                if (!string.IsNullOrEmpty(filePathSurvey))
                                {
                                    fileMergeName = Path.Combine(AppSettings.FolderTemp, bookingCruisePassenger.Booking.Number.ToString() + " - "
                                            + bookingCruisePassenger.Passenger.LastName + " " + bookingCruisePassenger.Passenger.FirstName + ".pdf");
                                    string fileMergePath = Path.Combine(AppSettings.FolderTemp, fileMergeName);
                                    Common.Pdf.MergePdf(filePathSurvey, fileMergePath, AppSettings.FolderTemp, bookingCruisePassenger);
                                    attachementList.Add(fileMergePath);
                                }

                                // Liste des destinataires
                                List<Recipient> recipients = new List<Recipient>
                                {
                                    new Recipient(AppSettings.AddressGroupTo, AppSettings.AddressGroupTo)
                                };
#if DEV || INTEGRATION || RECETTE
                                if (!string.IsNullOrWhiteSpace(AppSettings.AddressDebug))
                                {
                                    recipients.Add(new Recipient(AppSettings.AddressDebug, AppSettings.AddressDebug));
                                }
#endif
                                // Envoi du message
                                await MailServer.Send(new Mail()
                                {
                                    Body = (MailServer.ReplaceTags(bodyValue, bookingCruisePassenger.Booking, bookingCruisePassenger, null)).Replace(AppSettings.TagGroup, bookingCruisePassenger.Booking.GroupName),
                                    From = AppSettings.AddressFrom,
                                    Recipients = recipients,
                                    Subject = (MailServer.ReplaceTags(message.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, null)).Replace(AppSettings.TagGroup, bookingCruisePassenger.Booking.GroupName),
                                    Attachments = attachementList.Count > 0 ? attachementList : null
                                });
                            }

                            // Mise à jour des passagers relancer
                            (listBookingPassengerSend.Concat(listBookingPassengerClosed).ToList()).ForEach(tmp_bcp =>
                            {
                                BookingCruisePassenger bcp = _shoreEntities.BookingCruisePassenger.Find(tmp_bcp.Id);
                                int nbSent = bcp.Passenger.SentCount + 1;
                                bcp.Passenger.SentCount = nbSent;
                                bcp.Passenger.SentDate = Now;
                                bcp.Passenger.ModificationDate = Now;
                                bcp.Passenger.Editor = CurrentUser;

                                if (bcp.Passenger.IdStatus == Constants.SHORE_STATUS_QM_NOT_SENT)
                                {
                                    bcp.Passenger.IdStatus = Constants.SHORE_STATUS_QM_SENT;
                                }
                            //Update QmReceived to Qm sent
                            if (bcp.Passenger.IdStatus == Constants.SHORE_STATUS_QM_RECEIVED && statusNeedToBeUpdate)
                                {
                                    bcp.Passenger.IdStatus = Constants.SHORE_STATUS_QM_SENT;
                                }

                            // Maj booking
                            bcp.Booking.IdSurveyLanguage = language.Id;
                            });
                            _shoreEntities.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Group email address not given");
                    }
                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Send, CurrentUser, "Send Group relaunch for group : " + bookingCruisePassenger.Booking.GroupName);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Send, CurrentUser, "Send Group relaunch for group : " + bookingCruisePassenger.Booking.GroupName + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
                finally
                {
                    // Suppression du questionnaire => Obligation de l'enregistrer sur le disque pour le mettre en pièce jointe du mail
                    FileManager.FileDelete(AppSettings.FolderTemp, fileMergeName);
                }
            }
            return true;
        }
        #endregion

        #region GetBookingNumber
        public int GetBookingNumber(int idBooking)
        {
            return _shoreEntities.Booking.Find(idBooking).Number;
        }
        #endregion

        #region IsGroupPassenger
        public bool IsGroupPassenger(int idBooking)
        {
            return _shoreEntities.Booking.Find(idBooking).IsGroup;
        }
        #endregion
    }
    #endregion
}