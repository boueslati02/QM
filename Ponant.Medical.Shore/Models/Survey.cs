namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Helpers;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #region Modèles des vues

    #region SurveyViewModel
    /// <summary>
    /// Représentation pour la vue des questionnaires
    /// </summary>
    public class SurveyViewModel
    {
        /// <summary>
        /// Identifiant du questionnaire
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du questionnaire
        /// </summary>
        [Required]
        [MaxLength(64)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicateur de questionnaire médical
        /// </summary>
        [Display(Name = "Medical advice")]
        public bool MedicaleAdvice { get; set; }
    }
    #endregion

    #region CreateSurveyViewModel
    /// <summary>
    /// Représentation pour la vue de création d'un questionnaire
    /// </summary>
    public class CreateSurveyViewModel : SurveyViewModel
    { }
    #endregion

    #region EditSurveyViewModel
    /// <summary>
    /// Représentation pour la vue d'édition d'un questionnaire
    /// </summary>
    public class EditSurveyViewModel : SurveyViewModel
    { }
    #endregion

    #endregion

    #region Gestion des questionnaires
    /// <summary>
    /// Classe de gestion des questionnaires
    /// </summary>
    public class SurveyClass : SharedClass
    {
        #region Properties & Constructors

        public SurveyClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region GetSurvey
        /// <summary>
        /// Retourne un questionnaire pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du questionnaire/param>
        /// <returns>Un critére</returns>
        public EditSurveyViewModel GetSurvey(int id)
        {
            EditSurveyViewModel model = new EditSurveyViewModel();

            Survey survey = _shoreEntities.Survey.Find(id);
            if (survey != null)
            {
                model.Id = survey.Id;
                model.Name = survey.Name;
                model.MedicaleAdvice = survey.MedicalAdvice;
            }
            return model;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'un questionnaire
        /// </summary>
        /// <param name="model">Questionnaire à créer</param>
        /// <returns>Id du questionnaire ajouter, null en cas d'erreur</returns>
        public int? Create(CreateSurveyViewModel model)
        {
            int? surveyId = null;

            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            try
            {
                Survey survey = new Survey
                {
                    Name = model.Name,
                    MedicalAdvice = model.MedicaleAdvice,
                    Creator = CurrentUser,
                    CreationDate = Now,
                    Editor = CurrentUser,
                    ModificationDate = Now
                };

                _shoreEntities.Survey.Add(survey);
                _shoreEntities.SaveChanges();

                surveyId = survey.Id;
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Survey Id : " + surveyId.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Survey Id : " + surveyId.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
            return surveyId;
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un questionnaire
        /// </summary>
        /// <param name="model">Questionnaire à modifier</param>
        public void Edit(EditSurveyViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;
            try
            {
                Survey survey = _shoreEntities.Survey.Find(model.Id);
                survey.Name = model.Name;
                survey.MedicalAdvice = model.MedicaleAdvice;
                survey.Editor = CurrentUser;
                survey.ModificationDate = Now;
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Survey Id : " + model.Id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Survey Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion

        #region IsInUse
        /// <summary> Contrôle avant suppression
        ///  Définit si des critéres empéche la suppression du questionnaire
        /// </summary>
        /// <param name="id">Id du questionnaire</param>
        /// <returns>Vrai si le questionnaire est supprimable, faux sinon</returns>
        public bool IsInUse(int id)
        {
            int nbUse = (from criterion in _shoreEntities.CruiseCriterion where criterion.IdSurvey.Equals(id) select criterion.Id).Count();
            return nbUse > 0;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un questionnaire
        /// </summary>
        /// <param name="id">Identifiant du questionnaire</param>
        public void Delete(int id)
        {
            try
            {
                Survey survey = _shoreEntities.Survey.Find(id);

                if (survey.Language != null)
                {
                    foreach (Language lang in survey.Language) // Suppression des fichiers physiques
                    {
                        FileManager.FileDelete(AppSettings.FolderSurveyIndividual, (lang.Id.ToString() + lang.IndividualSurveyFileName));
                        FileManager.FileDelete(AppSettings.FolderSurveyGroup, (lang.Id.ToString() + lang.GroupSurveyFileName));
                        FileManager.FileDelete(AppSettings.FolderMailIndividual, (lang.Id.ToString() + lang.IndividualSurveyMail));
                        FileManager.FileDelete(AppSettings.FolderMailGroup, (lang.Id.ToString() + lang.GroupSurveyMail));
                    }
                }

                _shoreEntities.Language.RemoveRange(from lang in _shoreEntities.Language where lang.IdSurvey.Equals(survey.Id) select lang);
                _shoreEntities.Survey.Remove(survey);
                _shoreEntities.SaveChanges();

                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Survey, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Survey Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Survey, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Survey Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion
    }

    #endregion
}