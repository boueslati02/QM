namespace Ponant.Medical.Shore.Models
{
    using Foolproof;
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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using static Ponant.Medical.Shore.Models.MedicalClass;

    #region Modèles des vues

    #region MedicalViewModel
    /// <summary>
    /// Représentation pour la liste des passagers pour la partie médical
    /// </summary>
    public class MedicalViewModel
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
    }
    #endregion

    #region EditMedicalViewModel
    /// <summary>
    /// Représentation pour l'édition d'un passager pour la partie médical
    /// </summary>
    public class EditMedicalViewModel
    {
        /// <summary>
        /// Identifiant du passager
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de la croisière
        /// </summary>
        public int IdCruise { get; set; }

        /// <summary>
        /// Nom du passager
        /// </summary>
        public string Name { get; set; }
    }
    #endregion

    #region GetDocumentsViewModel
    /// <summary>
    /// Représentation pour l'affichage de la liste des documents
    /// </summary>
    public class GetDocumentsMedicalViewModel : EditMedicalViewModel
    { }
    #endregion

    #region EditAdviceViewModel
    /// <summary>
    /// Représentation pour l'édition de l'avis du médecin
    /// </summary>
    public class EditAdviceViewModel : EditMedicalViewModel
    {
        /// <summary>
        /// Identifiant de l'avis médical
        /// </summary>
        [Required]
        [Display(Name = "Advice")]
        public int IdAdvice { get; set; }

        /// <summary>
        /// Liste des type de document attendu
        /// </summary>
        [RequiredIf("IdAdvice", Constants.ADVICE_WAITING_FOR_CLARIFICATION, ErrorMessage = "The {0} field is required.")]
        [Display(Name = "Types of documents")]
        public List<int> DocumentsTypes { get; set; }

        /// <summary>
        /// Liste des motif d'avis défavorable
        /// </summary>
        [RequiredIf("IdAdvice", Constants.ADVICE_UNFAVORABLE_OPINION, ErrorMessage = "The {0} field is required.")]
        [Display(Name = "Case of unfavorable opinion")]
        public List<int> UnfavorableOpinion { get; set; }

        /// <summary>
        /// Liste des restrictions
        /// </summary>
        //[RequiredIf("IdAdvice", Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS, ErrorMessage = "The {0} field is required.")]
        [RestrictionsAttribute (ErrorMessage = "The {0} field is required.")]
        [Display(Name = "List of restrictions")]
        public List<int> RestrictionsTypes { get; set; }

        /// <summary>
        /// Liste des type de personne avec restrictions
        /// </summary>
        //[RequiredIf("IdAdvice", Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS, ErrorMessage = "The {0} field is required.")]
        [RestrictionsAttribute(ErrorMessage = "The {0} field is required.")]
        [Display(Name = "People with restriction")]
        public List<int> RestrictionsPeople { get; set; }

        /// <summary>
        /// Commentaire médical
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }
    #endregion

    #endregion

    #region Gestion des passagers partie médical
    /// <summary>
    /// Classe de gestion des passagers partie médical
    /// </summary>
    public class MedicalClass : SharedPassengerClass
    {
        #region Properties & Constructors

        private readonly ApplicationDbContext _applicationDbContext;

        public MedicalClass(IShoreEntities shoreEntities, ApplicationDbContext applicationDbContext) : base(shoreEntities)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion

        #region GetCruiseData
        /// <summary>
        /// Retourne les données de la croisiére pour affichage
        /// </summary>
        /// <param name="id">Identifiant de la croisière</param>
        /// <returns>Model de vue de la croisière</returns>
        public MedicalViewModel GetCruiseData(int id)
        {
            MedicalViewModel model = new MedicalViewModel();

            // Liste de tous les passagers de la croisière
            List<vPassengerShore> passengerShore = (from ps in _shoreEntities.vPassengerShore where (ps.IsEnabled) && ps.IdCruise.Equals(id) orderby ps.GroupName, ps.Id select ps).Distinct().ToList();

            Cruise cruise = _shoreEntities.Cruise.Find(id);
            model.IdCruise = cruise.Id;
            model.CruiseCode = cruise.Code;
            model.NbPassenger = (from ps in passengerShore select ps.Id).Distinct().Count();
            model.NbQMShore = (from ps in passengerShore
                               where (ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_RECEIVED) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_CLOSED) ||
                                ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_NEW_DOCUMENTS) || ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_IN_PROGRESS) ||
                                ps.IdStatus.Equals(Constants.SHORE_STATUS_QM_INCOMPLETE))
                               select ps.Id).Distinct().Count();
            model.NbQMBoard = (from ps in passengerShore where ps.IsExtract select ps.Id).Distinct().Count();
            model.ShipName = cruise.LovShip.Name;
            model.NbQMTreat = (from ps in passengerShore
                               where (ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS) ||
                                ps.IdAdvice.Equals(Constants.ADVICE_UNFAVORABLE_OPINION) || ps.IdAdvice.Equals(Constants.ADVICE_WAITING_FOR_CLARIFICATION))
                               select ps.Id).Distinct().Count();
            model.NbQMDowloadCruise = (from ps in passengerShore where ps.IsDownloaded select ps.Id).Distinct().Count();

            return model;
        }
        #endregion

        #region GetDoctorList
        /// <summary>
        /// Retourne la liste des docteurs
        /// </summary>
        /// <returns>Liste des docteur</returns>
        public List<SelectListItem> GetDoctorList()
        {
            return _applicationDbContext.vUsers
                .Where(u => u.RoleId == Constants.ROLE_ID_DOCTOR || u.RoleId == Constants.ROLE_ID_BOARD)
                .OrderBy(u => u.LastName)
                .Select(u => 
                    new SelectListItem()
                    {
                        Text = u.LastName,
                        Value = u.Id
                    }).ToList();
        }
        #endregion

        #region GetPassengerInformationList
        /// <summary>
        /// Retourne la liste des information associé au passager
        /// </summary>
        /// <returns>Liste des informations</returns>
        public List<string> GetPassengerInformationList(int idPassenger)
        {
            Passenger passenger = _shoreEntities.Passenger.Find(idPassenger);

            List<string> listInformations = (from info in passenger.Information
                                             orderby info.LovDocumentType.Name ascending
                                             select info.LovDocumentType.Name).Distinct().ToList();

            return listInformations;
        }
        #endregion

        #region GetPassengerForDocument
        /// <summary>
        /// Retourne un passagers pour la vue d'affichage des documents
        /// </summary>
        /// <param name="id">Identifiant du passager/param>
        /// <returns>Une représentation pour le passager</returns>
        public GetDocumentsMedicalViewModel GetPassengerForDocument(int id)
        {
            GetDocumentsMedicalViewModel model = new GetDocumentsMedicalViewModel();

            Passenger passager = _shoreEntities.Passenger.Find(id);
            if (passager != null)
            {
                model.Id = passager.Id;
                model.Name = passager.FirstName + " " + passager.LastName;
            }

            return model;
        }
        #endregion

        #region GetPassengerForAdvice
        /// <summary>
        /// Retourne un passagers pour la vue de modification d'avis médical
        /// </summary>
        /// <param name="id">Identifiant du passager/param>
        /// <returns>Une représentation pour le passager</returns>
        public EditAdviceViewModel GetPassengerForAdvice(int id)
        {
            EditAdviceViewModel model = new EditAdviceViewModel();

            Passenger passager = _shoreEntities.Passenger.Find(id);
            if (passager != null)
            {
                model.Id = passager.Id;
                model.Name = passager.FirstName + " " + passager.LastName;
            }

            return model;
        }
        #endregion

        #region EditAdvice
        /// <summary>
        /// Modifie l'avis médical d'un passager
        /// </summary>
        /// <param name="model">Avis médical à modifier</param>
        /// <returns>Vrai si l'email est envoyé, faux sinon</returns>
        public async Task<bool> EditAdvice(EditAdviceViewModel model)
        {
            bool result = true;

            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string currentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                BookingCruisePassenger bookingCruisePassenger = null;
                try
                {
                    Passenger passenger = _shoreEntities.Passenger.Find(model.Id);
                    passenger.IdAdvice = model.IdAdvice;
                    passenger.TreatmentDate = Now;
                    passenger.Doctor = UserHelper.GetUserId(currentUser);
                    passenger.Review = model.Comments;
                    passenger.Editor = currentUser;
                    passenger.ModificationDate = Now;
                    _shoreEntities.SaveChanges();

                    bookingCruisePassenger = _shoreEntities.BookingCruisePassenger.Where(bcp => bcp.IdCruise.Equals(model.IdCruise) && bcp.IdPassenger.Equals(model.Id)).FirstOrDefault();
                    _shoreEntities.Information.RemoveRange(passenger.Information); // Suppression des anciennes informations

                    switch (model.IdAdvice) // Selection pour l'envoi de l'email correspondant
                    {
                        case Constants.ADVICE_UNFAVORABLE_OPINION:
                            List<Information> unfavorableOpinion = SetInformationList(model.UnfavorableOpinion, passenger.Id, currentUser, Now);
                            if (unfavorableOpinion != null)
                            {
                                _shoreEntities.Information.AddRange(unfavorableOpinion);
                            }
                            passenger.IdStatus = Constants.SHORE_STATUS_QM_CLOSED;
                            _shoreEntities.SaveChanges();

                            break;
                        case Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS:
                            List<Information> restrictionsTypes = SetInformationList(model.RestrictionsTypes, passenger.Id, currentUser, Now);
                            if (restrictionsTypes != null)
                            {
                                _shoreEntities.Information.AddRange(restrictionsTypes);
                            }
                            _shoreEntities.SaveChanges();

                            List<Information> restrictionsPeople = SetInformationList(model.RestrictionsPeople, passenger.Id, currentUser, Now);
                            if (restrictionsPeople != null)
                            {
                                _shoreEntities.Information.AddRange(restrictionsPeople);
                            }
                            passenger.IdStatus = Constants.SHORE_STATUS_QM_CLOSED;
                            _shoreEntities.SaveChanges();

                            break;
                        case Constants.ADVICE_WAITING_FOR_CLARIFICATION:
                            List<Information> documentsTypes = SetInformationList(model.DocumentsTypes, passenger.Id, currentUser, Now);
                            if (documentsTypes != null)
                            {
                                _shoreEntities.Information.AddRange(documentsTypes);
                            }
                            passenger.IdStatus = Constants.SHORE_STATUS_QM_INCOMPLETE;
                            int nbSent = bookingCruisePassenger.Passenger.SentCount + 1;
                            passenger.SentCount = nbSent;
                            passenger.SentDate = Now;
                            _shoreEntities.SaveChanges();

                            List<Passenger> passengersList = new List<Passenger>() { passenger };
                            Dictionary<string, string> agenceNameEmail = (bookingCruisePassenger.Booking.IdAgency != 0 && bookingCruisePassenger.Booking.Agency != null) ? new Dictionary<string, string>() { { bookingCruisePassenger.Booking.Agency.Name.ToString(), bookingCruisePassenger.Booking.Agency.Email.ToString() } } : null;
                            CounterRelaunchResult resultCounter = await SendRelaunchByPassenger(passengersList, model.IdCruise, agenceNameEmail, true);
                            if (resultCounter != null && resultCounter.NbErrorMail > 0)
                            {
                                result = false;
                            }

                            break;
                        case Constants.ADVICE_FAVORABLE_OPINION:
                            passenger.IdStatus = Constants.SHORE_STATUS_QM_CLOSED;
                            _shoreEntities.SaveChanges();

                            break;
                    }

                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Passenger, LogManager.LogAction.Edit, currentUser, "Edit Advice passenger Id : " + model.Id.ToString(), (bookingCruisePassenger != null ? (int?)bookingCruisePassenger.Booking.Number : null));
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Passenger, LogManager.LogAction.Edit, currentUser, "Edit Advice passenger Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")", (bookingCruisePassenger != null ? (int?)bookingCruisePassenger.Booking.Number : null));
                    throw;
                }
            }
            return result;
        }
        #endregion

        #region Private

        #region SetInformationList
        /// <summary>
        /// Genere une liste d'information a inserer
        /// </summary>
        /// <param name="informationIdList">List des identifiant d'information</param>
        /// <param name="IdPassenger">Identifiant du passagers</param>
        /// <param name="currentUser">Utilisateur courrant</param>
        /// <param name="now">Date courrante</param>
        /// <returns>Liste d'informations</returns>
        private List<Information> SetInformationList(List<int> informationIdList, int IdPassenger, string currentUser, DateTime now)
        {
            List<Information> listInformation = null;
            if (informationIdList != null)
            {
                listInformation = new List<Information>();
                foreach (int value in informationIdList)
                {
                    Information information = new Information
                    {
                        IdPassenger = IdPassenger,
                        IdInformation = value,
                        Creator = currentUser,
                        CreationDate = now,
                        Editor = currentUser,
                        ModificationDate = now
                    };
                    listInformation.Add(information);
                }
            }
            return listInformation;
        }
        #endregion

        #region SendEmailUnfavorableAdvice
        /// <summary>
        /// Envoi de l'email d'alerte lors d'un avis défavorable
        /// </summary>
        /// <param name="bookingCruisePassenger">Instance du passager sur la croisière</param>
        /// <returns></returns>
        private async Task<bool> SendEmailUnfavorableAdvice(BookingCruisePassenger bookingCruisePassenger)
        {
            string messagePath = FileManager.FileGetPath(AppSettings.FolderMail, AppSettings.MailUnfavorableAdvice);

            if (!string.IsNullOrEmpty(messagePath))
            {
                using (Storage.Message message = new Storage.Message(messagePath))
                {
                    Mail mail = new Mail()
                    {
                        Body = MailServer.ReplaceTags(message.BodyHtml, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger),
                        From = AppSettings.AddressFrom,
                        Subject = MailServer.ReplaceTags(message.Subject, bookingCruisePassenger.Booking, bookingCruisePassenger, bookingCruisePassenger.Passenger),
                    };

                    // Création de la liste des destinataires
                    List<Recipient> recipients = new List<Recipient>();
                    if (bookingCruisePassenger.Booking.IsGroup) // Traitement pour les passagers en groupe
                    {
                        if (!string.IsNullOrEmpty(AppSettings.AddressGroupTo))
                        {
                            MailServer.SendMailToRecipient(mail, new Recipient(AppSettings.AddressGroupTo, AppSettings.AddressGroupTo), AppSettings.AddressDebug);
                        }
                    }
                    else // Traitement pour les passagers individuels
                    {
                        if (!string.IsNullOrEmpty(AppSettings.AddressBookingTo))
                        {
                            MailServer.SendMailToRecipient(mail, new Recipient(AppSettings.AddressBookingTo, AppSettings.AddressBookingTo), AppSettings.AddressDebug);
                        }
                    }

                    if (bookingCruisePassenger.Booking.IdAgency != 0 && bookingCruisePassenger.Booking.Agency != null) // Ajout de l'email de l'agence comme destinataire
                    {
                        if (!string.IsNullOrEmpty(bookingCruisePassenger.Booking.Agency.Email))
                        {
                            MailServer.SendMailToRecipient(mail, new Recipient(bookingCruisePassenger.Booking.Agency.Name, bookingCruisePassenger.Booking.Agency.Email), AppSettings.AddressDebug);
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region RestrictionsAttribute
        /// <summary>
        /// Classe d'attribut Custom pour la validation des restrictions
        /// </summary>
        public class RestrictionsAttribute : ModelAwareValidationAttribute
        {
            //this is needed to register this attribute with foolproof's validator adapter
            static RestrictionsAttribute() { Register.Attribute(typeof(RestrictionsAttribute)); }

            public override bool IsValid(object value, object container)
            {
                EditAdviceViewModel model = (EditAdviceViewModel)container;

                if (model.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS)
                    && model.RestrictionsTypes == null
                    && model.RestrictionsPeople == null)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}