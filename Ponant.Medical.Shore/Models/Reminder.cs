namespace Ponant.Medical.Shore.Models
{
    using Foolproof;
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #region Modèles des vues

    #region ReminderViewModel
    /// <summary>
    /// Représentation pour la vue des rappel
    /// </summary>
    public class ReminderViewModel
    {
        /// <summary>
        /// Identifiant du premier rappel
        /// </summary>
        public int FirstReminderId { get; set; }

        /// <summary>
        /// 1er rappel actif
        /// </summary>
        [Display(Name = "1st reminder")]
        public bool FirstReminderEnabled { get; set; }

        /// <summary>
        ///  Nombre de jours du 1er rappel
        /// </summary>
        [Display(Name = "days before departure")]
        [RequiredIfTrue("FirstReminderEnabled")]
        public int FirstReminderDays { get; set; }

        /// <summary>
        /// Identifiant du 2eme rappel
        /// </summary>
        public int SecondReminderId { get; set; }

        /// <summary>
        /// 2eme rappel actif
        /// </summary>
        [Display(Name = "2nd reminder")]
        public bool SecondReminderEnabled { get; set; }

        /// <summary>
        /// Nombre de jours du 2eme rappel
        /// </summary>
        [Display(Name = "days before departure")]
        [RequiredIfTrue("SecondReminderEnabled")]
        [LessThanOrEqualTo("FirstReminderDays", ErrorMessage = "2nd reminder must be less than 1st reminder.")]
        public int SecondReminderDays { get; set; }

        /// <summary>
        /// Identifiant du 3eme rappel
        /// </summary>
        public int ThirdReminderId { get; set; }

        /// <summary>
        /// 3eme rappel actif
        /// </summary>
        [Display(Name = "3rd reminder")]
        public bool ThirdReminderEnabled { get; set; }

        /// <summary>
        /// Nombre de jours du 3eme rappel
        /// </summary>
        [Display(Name = "days prior to departure")]
        [RequiredIfTrue("ThirdReminderEnabled")]
        [LessThanOrEqualTo("SecondReminderDays", ErrorMessage = "3rd reminder must be less than 2nd reminder.")]
        public int ThirdReminderDays { get; set; }
    }
    #endregion

    #endregion

    #region Gestion des rappels
    /// <summary>
    /// Classe de gestion des rappels
    /// </summary>
    public class ReminderClass : SharedClass
    {
        #region Properties & Constructors

        public ReminderClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region Edit
        /// <summary>
        /// Modifie les rappels
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void Edit(ReminderViewModel model)
        {
            try
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;

                Reminder FirstReminder = _shoreEntities.Reminder
                    .Find(model.FirstReminderId);

                if (FirstReminder != null)
                {
                    if ((FirstReminder.Enabled != model.FirstReminderEnabled) || (FirstReminder.Length != model.FirstReminderDays))
                    {
                        FirstReminder.Enabled = model.FirstReminderEnabled;
                        FirstReminder.Length = model.FirstReminderDays;
                        FirstReminder.ModificationDate = Now;
                        FirstReminder.Editor = CurrentUser;
                    }
                }

                Reminder SecondReminder = _shoreEntities.Reminder
                    .Find(model.SecondReminderId);

                if (SecondReminder != null)
                {
                    if ((SecondReminder.Enabled != model.SecondReminderEnabled) || (SecondReminder.Length != model.SecondReminderDays))
                    {
                        SecondReminder.Enabled = model.SecondReminderEnabled;
                        SecondReminder.Length = model.SecondReminderDays;
                        SecondReminder.ModificationDate = Now;
                        SecondReminder.Editor = CurrentUser;
                    }
                }

                Reminder ThirdReminder = _shoreEntities.Reminder
                    .Find(model.ThirdReminderId);

                if (ThirdReminder != null)
                {
                    if ((ThirdReminder.Enabled != model.ThirdReminderEnabled) || (ThirdReminder.Length != model.ThirdReminderDays))
                    {
                        ThirdReminder.Enabled = model.ThirdReminderEnabled;
                        ThirdReminder.Length = model.ThirdReminderDays;
                        ThirdReminder.ModificationDate = Now;
                        ThirdReminder.Editor = CurrentUser;
                    }
                }

                _shoreEntities.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Reminder, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Reminders");
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Reminder, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Reminders (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region GetReminders
        /// <summary>
        /// Retourne les 3 rappels
        /// </summary>
        /// <returns></returns>
        public ReminderViewModel GetReminders()
        {
            ReminderViewModel model = new ReminderViewModel();

            Reminder FirstReminder = _shoreEntities.Reminder.FirstOrDefault(r => r.Order == 1);
            if (FirstReminder != null)
            {
                model.FirstReminderId = FirstReminder.Id;
                model.FirstReminderEnabled = FirstReminder.Enabled;
                model.FirstReminderDays = FirstReminder.Length;
            }

            Reminder SecondReminder = _shoreEntities.Reminder.FirstOrDefault(r => r.Order == 2);
            if (SecondReminder != null)
            {
                model.SecondReminderId = SecondReminder.Id;
                model.SecondReminderEnabled = SecondReminder.Enabled;
                model.SecondReminderDays = SecondReminder.Length;
            }

            Reminder ThirdReminder = _shoreEntities.Reminder.FirstOrDefault(r => r.Order == 3);
            if (ThirdReminder != null)
            {
                model.ThirdReminderId = ThirdReminder.Id;
                model.ThirdReminderEnabled = ThirdReminder.Enabled;
                model.ThirdReminderDays = ThirdReminder.Length;
            }

            return model;
        }
        #endregion
    }

    #endregion
}