namespace Ponant.Medical.Shore.Models
{
    using MsgReader.Outlook;
    using Ponant.Medical.Common;
    using Ponant.Medical.Common.MailServer;
    using Ponant.Medical.Data.Shore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    #region Modèles des vues

    #region CruiseAssignmentViewModel
    /// <summary>
    /// Formulaire de création des assignations des croisières
    /// </summary>
    public class CruiseAssignmentViewModel
    {
        /// <summary>
        /// Identifiant du bateau
        /// </summary>
        [Required]
        public int Ship { get; set; }

        /// <summary>
        /// Nom des croisières
        /// </summary>
        [Required]
        public string Cruises { get; set; }

        /// <summary>
        /// Date limite
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
    }
    #endregion

    #region CreateCruiseAssignmentViewModel
    public class CreateCruiseAssignmentViewModel : CruiseAssignmentViewModel
    { }
    #endregion

    #region EditCruiseAssignmentViewModel
    public class EditCruiseAssignmentViewModel : CruiseAssignmentViewModel
    {
        /// <summary>
        /// identifiant de l'assignation de la croisière
        /// </summary>
        public int Id { get; set; }
    }
    #endregion

    #endregion

    #region Gestion des assignations des croisières
    /// <summary>
    /// Classe de gestion des assignations des croisières
    /// </summary>
    public class CruiseAssignmentClass : SharedClass
    {
        #region Properties & Constructors
        public CruiseAssignmentClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }
        #endregion

        #region GetCruiseAssignment
        /// <summary>
        /// Retourne une assignation des croisières
        /// </summary>
        /// <param name="id">Identifiant du droit d'accès/param>
        /// <returns>Un droits d'accès</returns>
        public EditCruiseAssignmentViewModel GetCruiseAssignment(int id)
        {
            EditCruiseAssignmentViewModel model = new EditCruiseAssignmentViewModel();
            Assignment assignment = _shoreEntities.Assignment.Find(id);

            if (assignment != null)
            {
                model.Ship = assignment.IdShip;
                model.Cruises = assignment.Cruises;
                model.Deadline = assignment.Deadline;
            }
            return model;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'une assignation des croisières
        /// </summary>
        /// <param name="model">Création de l'assignation</param>
        /// <returns></returns>
        public async Task _Create(CreateCruiseAssignmentViewModel model)
        {
            string currentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            try
            {
                Assignment assignment = new Assignment
                {
                    IdShip = model.Ship,
                    Cruises = model.Cruises,
                    Deadline = model.Deadline.Value,
                    Creator = currentUser,
                    CreationDate = Now,
                    Editor = currentUser,
                    ModificationDate = Now
                };

                _shoreEntities.Assignment.Add(assignment);
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Assignment, LogManager.LogAction.Add, currentUser, "Add cruise assignment");

                // Envoi du mail au navire assigné
                using (Storage.Message message = new Storage.Message(Path.Combine(AppSettings.FolderMail, AppSettings.MailAssignmentQm)))
                {
                    Lov lovShip = _shoreEntities.Lov.Find(assignment.IdShip);

                    Mail mail = new Mail()
                    {
                        Body = message.BodyHtml.Replace(AppSettings.TagShip, lovShip.Name).Replace(AppSettings.TagCruise, assignment.Cruises).Replace(AppSettings.TagDeadline, assignment.Deadline.ToShortDateString()),
                        From = AppSettings.AddressNoReply,
                        Recipients = message.Recipients.Select(r => new Recipient(r.DisplayName.Replace(AppSettings.TagShipCode, lovShip.Code), r.DisplayName.Replace(AppSettings.TagShipCode, lovShip.Code))).ToList(),
                        Subject = message.Subject
                    };

#if DEV || INTEGRATION || RECETTE
                    foreach (Recipient recipient in mail.Recipients)
                    {
                        recipient.Address = "x" + recipient.Address;
                        recipient.Name = "x" + recipient.Name;
                    }
                    mail.Recipients.Add(new Recipient(AppSettings.AddressDebug, AppSettings.AddressDebug));
#endif

                    await MailServer.Send(mail);
                }

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Assignment, LogManager.LogAction.Add, currentUser, "Send mail");
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Assignment, LogManager.LogAction.Add, currentUser, "Add cruise assignment" + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie une assignation des croisières
        /// </summary>
        /// <param name="model">assignation à modifier</param>
        public void Edit(EditCruiseAssignmentViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            try
            {
                Assignment assignment = _shoreEntities.Assignment.Find(model.Id);
                assignment.IdShip = model.Ship;
                assignment.Cruises = model.Cruises;
                assignment.Deadline = model.Deadline.Value;
                assignment.Editor = CurrentUser;
                assignment.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Assignment, LogManager.LogAction.Edit, CurrentUser, "Edit cruise assignment Id : " + model.Id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Assignment, LogManager.LogAction.Edit, CurrentUser, "Edit cruise assignment Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un assignation
        /// </summary>
        /// <param name="id">Identifiant de l'assignation</param>
        public void Delete(int id)
        {
            try
            {
                Assignment assignment = _shoreEntities.Assignment.Find(id);
                _shoreEntities.Assignment.Remove(assignment);
                _shoreEntities.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Assignment, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete cruise assignment Id : " + id);
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Assignment, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete cruise assignment Id : " + id + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion
    }
    #endregion
}