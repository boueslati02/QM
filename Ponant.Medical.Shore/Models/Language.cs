
namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #region Modèles des vues

    #region LanguageViewModel
    /// <summary>
    /// Représentation pour la vue des langages
    /// </summary>
    public class LanguageViewModel
    {
        /// <summary>
        /// Identifiant du langage
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du questionnaire associé
        /// </summary>
        public int IdSurvey { get; set; }

        /// <summary>
        /// Indicateur d'erreur lors de l'upload d'un fichier
        /// </summary>
        [Range(typeof(bool), "false", "false", ErrorMessage = "Can not be validated. Invalid file selected")]
        public bool UploadFileError { get; set; }

        /// <summary>
        /// Identifiant de la langue associée
        /// </summary>
        [Required]
        [Display(Name = "Language")]
        public int Langue { get; set; }

        /// <summary>
        /// Nom de fichier du questionnaire individuel
        /// </summary>
        [Display(Name = "Individual survey")]
        public string IndividualSurvey { get; set; }

        /// <summary>
        /// Nom de fichier pour l'email de réponse individuel automatique lors de la réception du QM
        /// </summary>
        [Display(Name = "Individual Automatic Response")]
        public string IndividualAutomaticResponse { get; set; }

        /// <summary>
        /// Nom de fichier pour l'email de réponse de Groupe automatique lors de la réception du QM
        /// </summary>
        [Display(Name = "Group Automatic Response")]
        public string GroupAutomaticResponse { get; set; }

        /// <summary>
        /// Nom de fichier du questionnaire de groupe
        /// </summary>
        [Display(Name = "Group survey")]
        public string GroupSurvey { get; set; }

        /// <summary>
        /// Nom de fichier pour les emails individuel
        /// </summary>
        [Display(Name = "Request individual mail")]
        public string IndividualMail { get; set; }

        /// <summary>
        /// Nom de fichier pour les emails de groupe
        /// </summary>
        [Display(Name = "Request group mail")]
        public string GroupMail { get; set; }

        /// <summary>
        /// Indique un langage par défaut
        /// </summary>
        [Remote("CheckDefaultValue", "Language", AdditionalFields = "IdSurvey, Id ", ErrorMessage = "Default language already exists", HttpMethod = "GET")]
        [Display(Name = "Default")]
        public bool DefaultLanguage { get; set; }
    }
    #endregion

    #region CreateLanguageViewModel
    /// <summary>
    /// Représentation pour la vue de création d'un langage
    /// </summary>
    public class CreateLanguageViewModel : LanguageViewModel
    { }
    #endregion

    #region EditLanguageViewModel
    /// <summary>
    /// Représentation pour la vue d'édition d'un langage
    /// </summary>
    public class EditLanguageViewModel : LanguageViewModel
    { }
    #endregion

    #endregion

    #region Gestion des langages
    /// <summary>
    /// Classe de gestion des langages
    /// </summary>
    public class LanguageClass : SharedClass
    {
        #region Properties & Constructors

        public LanguageClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region Enum FileLanguageType
        /// <summary>
        /// Enumeration du type de fichier
        /// </summary>
        public enum FileLanguageType
        {
            IndividualSurveyFileName = 1,
            GroupSurveyFileName = 2,
            IndividualSurveyMail = 3,
            GroupSurveyMail = 4,
            IndividualAutomaticResponse = 7,
            GroupAutomaticResponse = 8
        }
        #endregion

        #region GetLanguage
        /// <summary>
        /// Retourne un langage pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du langage/param>
        /// <returns>Un langage</returns>
        public EditLanguageViewModel GetLanguage(int id)
        {
            EditLanguageViewModel model = new EditLanguageViewModel();

            Language language = _shoreEntities.Language.Find(id);
            if (language != null)
            {
                model.Id = language.Id;
                model.IdSurvey = language.IdSurvey;
                model.Langue = language.IdLanguage;
                model.IndividualSurvey = language.IndividualSurveyFileName;
                model.IndividualAutomaticResponse = language.IndividualAutomaticResponse;
                model.GroupAutomaticResponse = language.GroupAutomaticResponse;
                model.GroupSurvey = language.GroupSurveyFileName;
                model.IndividualMail = language.IndividualSurveyMail;
                model.GroupMail = language.GroupSurveyMail;
                model.DefaultLanguage = language.IsDefault;
            }

            return model;
        }
        #endregion

        #region GetFileName
        /// <summary>
        /// Récupere le nom du fichier
        /// </summary>
        /// <param name="id">Id du langage concerné</param>
        /// <param name="fileType">Type du fichier voulu</param>
        /// <returns>Nom du fichier</returns>
        public string GetFileName(int id, FileLanguageType fileType)
        {
            string filename = null;
            switch (fileType)
            {
                case LanguageClass.FileLanguageType.IndividualSurveyFileName:
                    filename = _shoreEntities.Language.Find(id).IndividualSurveyFileName;
                    break;
                case LanguageClass.FileLanguageType.GroupSurveyFileName:
                    filename = _shoreEntities.Language.Find(id).GroupSurveyFileName;
                    break;
                case LanguageClass.FileLanguageType.IndividualSurveyMail:
                    filename = _shoreEntities.Language.Find(id).IndividualSurveyMail;
                    break;
                case LanguageClass.FileLanguageType.GroupSurveyMail:
                    filename = _shoreEntities.Language.Find(id).GroupSurveyMail;
                    break;
                case LanguageClass.FileLanguageType.IndividualAutomaticResponse:
                    filename = _shoreEntities.Language.Find(id).IndividualAutomaticResponse;
                    break;
                case LanguageClass.FileLanguageType.GroupAutomaticResponse:
                    filename = _shoreEntities.Language.Find(id).GroupAutomaticResponse;
                    break;
            }

            return filename;
        }
        #endregion

        #region IsValidDefault
        /// <summary>
        /// Retourne un indicateur d'utilisation du champ default
        /// </summary>
        /// <param name="DefaultValue">Valeur de default à tester</param>
        /// <param name="surveyId">Identifiant du questionnaire courant</param>
        /// <param name="id">Identifiant du langage courant</param>
        /// <returns>Vrai si aucun langage par défaut, faux sinon</returns>
        public bool IsValidDefault(bool DefaultValue, int? surveyId = null, int? id = null)
        {
            int result = 0;
            if (DefaultValue && surveyId.HasValue)
            {
                if ((id.HasValue) && (id != 0))
                {
                    result = (from l in _shoreEntities.Language where l.IdSurvey.Equals(surveyId.Value) && l.IsDefault && !l.Id.Equals(id.Value) select l.Id).Count();
                }
                else
                {
                    result = (from l in _shoreEntities.Language where l.IdSurvey.Equals(surveyId.Value) && l.IsDefault select l.Id).Count();
                }
            }
            return result <= 0;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'un langage
        /// </summary>
        /// <param name="model">Langage à créer</param>
        public void Create(CreateLanguageViewModel model)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                Language language = new Language();
                try
                {
                    language.IdSurvey = model.IdSurvey;
                    language.IdLanguage = model.Langue;
                    language.IsDefault = model.DefaultLanguage;
                    language.Creator = CurrentUser;
                    language.CreationDate = Now;
                    language.Editor = CurrentUser;
                    language.ModificationDate = Now;
                    _shoreEntities.Language.Add(language);
                    _shoreEntities.SaveChanges();

                    SaveLanguageFiles(ref language, model.IndividualSurvey, 1);
                    SaveLanguageFiles(ref language, model.GroupSurvey, 2);
                    SaveLanguageFiles(ref language, model.IndividualMail, 3);
                    SaveLanguageFiles(ref language, model.GroupMail, 4);
                    SaveLanguageFiles(ref language, model.IndividualAutomaticResponse, 7);
                    SaveLanguageFiles(ref language, model.GroupAutomaticResponse, 8);

                    _shoreEntities.SaveChanges();
                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Language, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Language Id : " + language.Id.ToString());
                }
                catch (Exception ex)
                {
                    DeleteOldFiles(language, model, true);
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Language, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Language Id : " + language.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un langage
        /// </summary>
        /// <param name="model">Langage à modifier</param>
        public void Edit(EditLanguageViewModel model)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                Language language = null;
                try
                {
                    language = _shoreEntities.Language.Find(model.Id);
                    language.IdLanguage = model.Langue;
                    language.IsDefault = model.DefaultLanguage;
                    language.Editor = CurrentUser;
                    language.ModificationDate = Now;
                    _shoreEntities.SaveChanges();

                    SaveLanguageFiles(ref language, model.IndividualSurvey, 1);
                    SaveLanguageFiles(ref language, model.GroupSurvey, 2);
                    SaveLanguageFiles(ref language, model.IndividualMail, 3);
                    SaveLanguageFiles(ref language, model.GroupMail, 4);
                    SaveLanguageFiles(ref language, model.IndividualAutomaticResponse, 7);
                    SaveLanguageFiles(ref language, model.GroupAutomaticResponse, 8);

                    _shoreEntities.SaveChanges();
                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Language, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Language Id : " + model.Id.ToString());
                }
                catch (Exception ex)
                {
                    DeleteOldFiles(language, model, false);
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Language, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Language Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region IsInUse
        /// <summary>Contrôle avant suppression
        ///  Définit si des booking empéche la suppression du langage
        /// </summary>
        /// <param name="id">Id du langage</param>
        /// <returns>Vrai si le langage est supprimable, faux sinon</returns>
        public bool IsInUse(int id)
        {
            int nbUse = (from bcp in _shoreEntities.BookingCruisePassenger
                         where DbFunctions.AddDays(bcp.Cruise.SailingDate, bcp.Cruise.SailingLengthDays) >= DateTime.Now &&
                         bcp.Booking.IdSurveyLanguage == id
                         select bcp.Booking.Id).Distinct().Count();
            return nbUse > 0;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un langage
        /// </summary>
        /// <param name="id">Identifiant du langage</param>
        /// <returns>Identifiant du questionnaire du langage, null sinon</returns>
        public int? Delete(int id)
        {
            int? surveyId = null;

            try
            {
                Language language = _shoreEntities.Language.Find(id);
                _shoreEntities.Language.Remove(language);
                _shoreEntities.SaveChanges();

                if (language != null) // Suppression des fichiers physiques
                {
                    FileManager.FileDelete(AppSettings.FolderSurveyIndividual, (language.Id.ToString() + language.IndividualSurveyFileName));
                    FileManager.FileDelete(AppSettings.FolderSurveyGroup, (language.Id.ToString() + language.GroupSurveyFileName));
                    FileManager.FileDelete(AppSettings.FolderMailIndividual, (language.Id.ToString() + language.IndividualSurveyMail));
                    FileManager.FileDelete(AppSettings.FolderMailGroup, (language.Id.ToString() + language.GroupSurveyMail));
                    FileManager.FileDelete(AppSettings.FolderMailIndividualAutomaticResponse, (language.Id.ToString() + language.IndividualAutomaticResponse));
                    FileManager.FileDelete(AppSettings.FolderMailGroupAutomaticResponse, (language.Id.ToString() + language.GroupAutomaticResponse));
                    surveyId = language.IdSurvey;
                }
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Language, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Language Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Language, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Language Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }

            return surveyId;
        }
        #endregion

        #region FileDelete
        /// <summary>
        /// Suppression d'un fichier de langue
        /// </summary>
        /// <param name="id">Identifiant du langage</param>
        /// <param name="fileType">Type de fichier a supprimer</param>
        /// <returns>Identifiant du questionnaire du langage, null sinon</returns>
        public int? FileDelete(int id, FileLanguageType fileType)
        {
            int? surveyId = null;

            try
            {
                string path = null;
                string filename = null;
                Language language = _shoreEntities.Language.Find(id);
                if (language != null)
                {
                    switch (fileType)
                    {
                        case LanguageClass.FileLanguageType.IndividualSurveyFileName:
                            path = AppSettings.FolderSurveyIndividual;
                            filename = language.IndividualSurveyFileName;
                            language.IndividualSurveyFileName = null;
                            break;
                        case LanguageClass.FileLanguageType.GroupSurveyFileName:
                            path = AppSettings.FolderSurveyGroup;
                            filename = language.GroupSurveyFileName;
                            language.GroupSurveyFileName = null;
                            break;
                        case LanguageClass.FileLanguageType.IndividualSurveyMail:
                            path = AppSettings.FolderMailIndividual;
                            filename = language.IndividualSurveyMail;
                            language.IndividualSurveyMail = null;
                            break;
                        case LanguageClass.FileLanguageType.GroupSurveyMail:
                            path = AppSettings.FolderMailGroup;
                            filename = language.GroupSurveyMail;
                            language.GroupSurveyMail = null;
                            break;
                        case LanguageClass.FileLanguageType.IndividualAutomaticResponse:
                            path = AppSettings.FolderMailIndividualAutomaticResponse;
                            filename = language.IndividualAutomaticResponse;
                            language.IndividualAutomaticResponse = null;
                            break;
                        case LanguageClass.FileLanguageType.GroupAutomaticResponse:
                            path = AppSettings.FolderMailGroupAutomaticResponse;
                            filename = language.GroupAutomaticResponse;
                            language.GroupAutomaticResponse = null;
                            break;

                    }
                    surveyId = language.IdSurvey;
                }

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
                {
                    _shoreEntities.SaveChanges();
                    FileManager.FileDelete(path, (language.Id.ToString() + filename)); // Suppression des fichiers physiques
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Language, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Language File Id : " + id.ToString());
                }
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Language, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Language File Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }

            return surveyId;
        }
        #endregion

        #region Private

        #region SaveLanguageFiles
        /// <summary>
        /// Enregistrement des fichier telecharger commun entre _Create et _Edit
        /// </summary>
        /// <param name="language">Langage concerné</param>
        /// <param name="files">Liste des fichier a enregistré</param>
        private void SaveLanguageFiles(ref Language language, string filename, int typeFile)
        {
            string destinationPath = null;
            string sourceFile = HttpContext.Current.User.Identity.Name;
            string originName = null;
            if (!string.IsNullOrEmpty(filename))
            {
                switch (typeFile)
                {
                    case 1:
                        sourceFile = sourceFile + "_IndividualSurvey" + filename;
                        destinationPath = AppSettings.FolderSurveyIndividual;
                        originName = language.IndividualSurveyFileName;
                        language.IndividualSurveyFileName = filename;
                        break;
                    case 2:
                        sourceFile = sourceFile + "_GroupSurvey" + filename;
                        destinationPath = AppSettings.FolderSurveyGroup;
                        originName = language.GroupSurveyFileName;
                        language.GroupSurveyFileName = filename;
                        break;
                    case 3:
                        sourceFile = sourceFile + "_IndividualMail" + filename;
                        destinationPath = AppSettings.FolderMailIndividual;
                        originName = language.IndividualSurveyMail;
                        language.IndividualSurveyMail = filename;
                        break;
                    case 4:
                        sourceFile = sourceFile + "_GroupMail" + filename;
                        destinationPath = AppSettings.FolderMailGroup;
                        originName = language.GroupSurveyMail;
                        language.GroupSurveyMail = filename;
                        break;
                    case 7:
                        sourceFile = sourceFile + "_IndividualAutomaticResponse" + filename;
                        destinationPath = AppSettings.FolderMailIndividualAutomaticResponse;
                        originName = language.IndividualAutomaticResponse;
                        language.IndividualAutomaticResponse = filename;
                        break;

                    case 8:
                        sourceFile = sourceFile + "_GroupAutomaticResponse" + filename;
                        destinationPath = AppSettings.FolderMailGroupAutomaticResponse;
                        originName = language.GroupAutomaticResponse;
                        language.GroupAutomaticResponse = filename;
                        break;
                }

                if (!string.IsNullOrEmpty(originName) && (originName != filename)) // Suppression de l'ancien fichier lors d'une modification
                {
                    FileManager.FileDelete(destinationPath, (language.Id.ToString() + originName));
                }

                FileManager.FileMove(AppSettings.FolderTemp, sourceFile, destinationPath, (language.Id.ToString() + filename), true);
            }
        }
        #endregion

        #region DeleteOldFiles
        /// <summary>
        /// Supprime les fichier transferer en cas d'erreur
        /// </summary>
        /// <param name="language">Langage en erreur</param>
        /// <param name="model">model de donnée en cours</param>
        /// <param name="deleteAll">Indicateur de suppression des fichier déplacer</param>
        private void DeleteOldFiles(Language language, LanguageViewModel model, bool deleteAll = false)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;

            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_IndividualSurvey" + model.IndividualSurvey), true);
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_GroupSurvey" + model.GroupSurvey), true);
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_IndividualMail" + model.IndividualMail), true);
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_GroupMail" + model.GroupMail), true);
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_IndividualAutomaticResponse" + model.IndividualAutomaticResponse), true);
            FileManager.FileDelete(AppSettings.FolderTemp, (CurrentUser + "_GroupAutomaticResponse" + model.GroupAutomaticResponse), true);

            if (deleteAll)
            {
                FileManager.FileDelete(AppSettings.FolderSurveyIndividual, (language.Id.ToString() + language.IndividualSurveyFileName));
                FileManager.FileDelete(AppSettings.FolderSurveyGroup, (language.Id.ToString() + language.GroupSurveyFileName));
                FileManager.FileDelete(AppSettings.FolderMailIndividual, (language.Id.ToString() + language.IndividualSurveyMail));
                FileManager.FileDelete(AppSettings.FolderMailGroup, (language.Id.ToString() + language.GroupSurveyMail));
                FileManager.FileDelete(AppSettings.FolderMailIndividualAutomaticResponse, (language.Id.ToString() + model.IndividualAutomaticResponse));
                FileManager.FileDelete(AppSettings.FolderMailGroupAutomaticResponse, (language.Id.ToString() + model.GroupAutomaticResponse));
            }
        }
        #endregion

        #endregion
    }
    #endregion
}