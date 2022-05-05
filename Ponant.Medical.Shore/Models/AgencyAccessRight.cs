namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #region Modèles des vues

    #region vAgencyAccessRight
    /// <summary>
    /// Vue de la grille des droits d'accès des agences
    /// </summary>
    public class vAgencyAccessRight
    {
        /// <summary>
        /// Identifiant du droit d'accès
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nom de l'agence
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// Code de la croisière
        /// </summary>
        public string CruiseCode { get; set; }

        /// <summary>
        /// Nom du groupe
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Numéro de la réservation
        /// </summary>
        public string BookingNumber { get; set; }
    }
    #endregion

    #region CreateAgencyAccessRightViewModel
    /// <summary>
    /// Formulaire de création des droits d'accès
    /// </summary>
    public class CreateAgencyAccessRightViewModel
    {
        /// <summary>
        /// Identifiant de l'agence
        /// </summary>
        [Required]
        [Display(Name = "Agency Name")]
        public int? IdAgency { get; set; }

        /// <summary>
        /// Nom de l'agence
        /// </summary>
        [Required]
        public string AgencyName { get; set; }

        /// <summary>
        /// Code de la croisière
        /// </summary>
        [Required]
        [StringLength(16)]
        [Display(Name = "Cruise Code")]
        public string CruiseCode { get; set; }

        /// <summary>
        /// Nom du groupe
        /// </summary>
        [Required]
        [StringLength(32)]
        [Display(Name = "Group ID")]
        public string GroupName { get; set; }

        /// <summary>
        /// Numéro de la réservation
        /// </summary>
        [Required]
        [Display(Name = "Booking Number")]
        public int? BookingNumber { get; set; }
    }
    #endregion

    #region EditAgencyAccessRightViewModel
    /// <summary>
    /// Formulaire d'édition des droits d'accès
    /// </summary>
    public class EditAgencyAccessRightViewModel : CreateAgencyAccessRightViewModel
    {
        /// <summary>
        /// Identifiant du droits d'accès
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
    #endregion

    #region Gestion des droits d'accès des agences
    /// <summary>
    /// Classe de gestion des droits d'accès des agences
    /// </summary>
    public class AgencyAccessRightClass : SharedClass
    {
        #region Properties & Constructors
        public AgencyAccessRightClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }
        #endregion

        #region GetAgencyAccessRight
        /// <summary>
        /// Retourne un droit d'accès pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du droit d'accès/param>
        /// <returns>Un droits d'accès</returns>
        public EditAgencyAccessRightViewModel GetAgencyAccessRight(int id)
        {
            EditAgencyAccessRightViewModel model = new EditAgencyAccessRightViewModel();
            AgencyAccessRight agencyAccessRight = _shoreEntities.AgencyAccessRight.Find(id);

            if (agencyAccessRight != null)
            {
                model.IdAgency = agencyAccessRight.IdAgency;
                model.AgencyName = agencyAccessRight.Agency.Name;
                model.CruiseCode = agencyAccessRight.CruiseCode;
                model.GroupName = agencyAccessRight.GroupName;
                model.BookingNumber = agencyAccessRight.BookingNumber;
            }
            return model;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'un droit d'accès
        /// </summary>
        /// <param name="model">Droits d'accès à créer</param>
        /// <returns></returns>
        public void _Create(CreateAgencyAccessRightViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            try
            {
                AgencyAccessRight AgencyAccessRight = new AgencyAccessRight
                {
                    IdAgency = model.IdAgency.Value,
                    CruiseCode = model.CruiseCode,
                    GroupName = model.GroupName,
                    BookingNumber = model.BookingNumber.Value,
                    Creator = CurrentUser,
                    CreationDate = Now,
                    Editor = CurrentUser,
                    ModificationDate = Now
                };

                _shoreEntities.AgencyAccessRight.Add(AgencyAccessRight);
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Add, CurrentUser, "Add agency access right", model.BookingNumber);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Add, CurrentUser, "Add agency access right" + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")", model.BookingNumber);
                throw;
            }
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un droit d'accès
        /// </summary>
        /// <param name="model">Droit d'accès à modifier</param>
        public void Edit(EditAgencyAccessRightViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            try
            {
                AgencyAccessRight AgencyAccessRight = _shoreEntities.AgencyAccessRight.Find(model.Id);
                AgencyAccessRight.BookingNumber = model.BookingNumber.Value;
                AgencyAccessRight.Editor = CurrentUser;
                AgencyAccessRight.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Edit, CurrentUser, "Edit agency access right Id : " + model.Id.ToString(), model.BookingNumber);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Edit, CurrentUser, "Edit agency access right Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")", model.BookingNumber);
                throw;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un droit d'accès
        /// </summary>
        /// <param name="id">Identifiant du droit d'accès</param>
        public void Delete(int id)
        {
            try
            {
                AgencyAccessRight AgencyAccessRight = _shoreEntities.AgencyAccessRight.Find(id);
                _shoreEntities.AgencyAccessRight.Remove(AgencyAccessRight);
                _shoreEntities.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete agency access right Id : " + id);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.AgencyAccessRight, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete agency access right Id : " + id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region GetPassengers
        public IEnumerable<Tuple<int, int>> GetPassengers(int bookingNumber)
        {
            List<Tuple<int, int>> ids = new List<Tuple<int, int>>();

            Booking booking = _shoreEntities.Booking.SingleOrDefault(b => b.Number == bookingNumber);

            if (booking != null)
            {
                List<BookingCruisePassenger> bookingCruisePassengers = booking.BookingCruisePassenger.ToList();

                if (bookingCruisePassengers != null)
                {
                    foreach (BookingCruisePassenger bookingCruisePassenger in bookingCruisePassengers)
                    {
                        ids.Add(new Tuple<int, int>(bookingCruisePassenger.IdPassenger, bookingCruisePassenger.IdCruise));
                    }
                }
            }

            return ids;
        }
        #endregion
    }
    #endregion

    #endregion
}