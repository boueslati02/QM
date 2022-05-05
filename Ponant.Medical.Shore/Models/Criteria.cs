namespace Ponant.Medical.Shore.Models
{
    using Ponant.Medical.Common;
    using Ponant.Medical.Data.Shore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #region Modèles des vues

    #region CriteriaViewModel
    /// <summary>
    /// Représentation pour la vue des critère
    /// </summary>
    public class CriteriaViewModel
    {
        /// <summary>
        /// Identifiant du critére
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ordre des critéres
        /// </summary>
        [Required]
        [Display(Name = "Order")]
        [Remote("CheckOrderValue", "Criteria", AdditionalFields = "Id", ErrorMessage = "Order value already exists", HttpMethod = "GET")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Order { get; set; }

        /// <summary>
        /// Identifiant du type de croisiére associé
        /// </summary>
        [Display(Name = "Type of cruise")]
        public int? CruiseType { get; set; }

        /// <summary>
        /// Identifiants des destinations associées
        /// </summary>
        [Display(Name = "Destination")]
        public List<int> Destinations { get; set; }

        /// <summary>
        /// Identifiants des bateaux associées
        /// </summary>
        [Display(Name = "Ship")]
        public List<int> Ships { get; set; }

        /// <summary>
        /// Nombre de jour
        /// </summary>
        [Display(Name = "Length")]
        public int? Length { get; set; }

        /// <summary>
        /// Code des bateaux
        /// </summary>
        [Display(Name = "Cruise")]
        [MaxLength(130)]
        public string Cruise { get; set; }

        /// <summary>
        /// Identifiants des activités associées
        /// </summary>
        [Display(Name = "Activity")]
        public string Activity { get; set; }

        /// <summary>
        /// Identifiant du questionnaire associé
        /// </summary>
        [Required]
        [Display(Name = "Survey")]
        public int Survey { get; set; }
    }
    #endregion

    #region CreateCriteriaViewModel
    /// <summary>
    /// Représentation pour la vue de création d'un critère
    /// </summary>
    public class CreateCriteriaViewModel : CriteriaViewModel
    { }
    #endregion

    #region EditCriteriaViewModel
    /// <summary>
    /// Représentation pour la vue d'édition d'un critère
    /// </summary>
    public class EditCriteriaViewModel : CriteriaViewModel
    { }
    #endregion

    #endregion

    #region Gestion des critères
    /// <summary>
    /// Classe de gestion des critères
    /// </summary>
    public class CriteriaClass : SharedClass
    {
        #region Properties & Constructors

        public CriteriaClass(IShoreEntities shoreEntities) : base(shoreEntities)
        { }

        #endregion

        #region GetSurveyList
        /// <summary>
        /// Retourne la liste des questionnaires
        /// </summary>
        /// <returns>Liste des questionnaires</returns>
        public List<SelectListItem> GetSurveyList()
        {
            return (from survey in _shoreEntities.Survey.AsEnumerable()
                    orderby survey.Name ascending
                    select new SelectListItem()
                    {
                        Text = survey.Name,
                        Value = survey.Id.ToString()
                    }).ToList();
        }
        #endregion

        #region GetCriterion
        /// <summary>
        /// Retourne un critére pour la vue de modification
        /// </summary>
        /// <param name="id">Identifiant du critére/param>
        /// <returns>Un critére</returns>
        public EditCriteriaViewModel GetCriterion(int id)
        {
            EditCriteriaViewModel model = new EditCriteriaViewModel();

            CruiseCriterion criterion = _shoreEntities.CruiseCriterion.Find(id);
            if (criterion != null)
            {
                model.Id = criterion.Id;
                model.Order = criterion.Order;
                model.CruiseType = criterion.IdCruiseType;
                model.Destinations = (from ccd in criterion.CruiseCriterionDestination select ccd.IdDestination).ToList();
                model.Ships = (from ccs in criterion.CruiseCriterionShip select ccs.IdShip).ToList();
                model.Length = criterion.Length;
                model.Cruise = criterion.Cruise;
                model.Activity = criterion.Activity;
                model.Survey = criterion.IdSurvey;
            }

            return model;
        }
        #endregion

        #region IsValidOrder
        /// <summary>
        /// Retourne un indicateur d'utilisation de l'ordre
        /// </summary>
        /// <param name="orderValue">Valeur de l'ordre à tester</param>
        /// <param name="forceCruiseCriterionId">Force la désactivation du test sur l'id du critére</param>
        /// <returns>Vrai si l'ordre n'existe pas, faux sinon</returns>
        public bool IsValidOrder(int orderValue, int? forceCruiseCriterionId = null)
        {
            int result = 0;

            if ((forceCruiseCriterionId.HasValue) && (forceCruiseCriterionId != 0))
            {
                result = (from cc in _shoreEntities.CruiseCriterion.AsEnumerable() where cc.Order.Equals(orderValue) && !cc.Id.Equals(forceCruiseCriterionId.Value) select cc.Id).Count();
            }
            else
            {
                result = (from cc in _shoreEntities.CruiseCriterion.AsEnumerable() where cc.Order.Equals(orderValue) select cc.Id).Count();
            }

            return result <= 0;
        }
        #endregion

        #region Create
        /// <summary>
        /// Création d'un critére
        /// </summary>
        /// <param name="model">Critére à créer</param>
        public void Create(CreateCriteriaViewModel model)
        {
            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                int? criterionId = null;
                string CurrentUser = HttpContext.Current.User.Identity.Name;
                DateTime Now = DateTime.Now;
                try
                {
                    // Ajout du criterion
                    CruiseCriterion criterion = new CruiseCriterion
                    {
                        IdSurvey = model.Survey,
                        IdCruiseType = model.CruiseType ?? 0,
                        Order = model.Order,
                        Cruise = model.Cruise,
                        Length = (model.Length.HasValue && model.Length == 0) ? null : model.Length,
                        Creator = CurrentUser,
                        CreationDate = Now,
                        Editor = CurrentUser,
                        ModificationDate = Now,
                        Activity = model.Activity
                    };
                    _shoreEntities.CruiseCriterion.Add(criterion);
                    _shoreEntities.SaveChanges();

                    criterionId = criterion.Id; // Id du criterion  inserer

                    // Insertion des destinations associées
                    if (model.Destinations != null)
                    {
                        foreach (int DestinationId in model.Destinations)
                        {
                            criterion.CruiseCriterionDestination.Add(new CruiseCriterionDestination()
                            {
                                IdCruiseCriterion = criterionId.Value,
                                IdDestination = DestinationId,
                                Creator = CurrentUser,
                                CreationDate = Now,
                                Editor = CurrentUser,
                                ModificationDate = Now
                            });
                        }
                    }


                    // Insertion des bateaux associés
                    if (model.Ships != null)
                    {
                        foreach (int ShipId in model.Ships)
                        {
                            criterion.CruiseCriterionShip.Add(new CruiseCriterionShip()
                            {
                                IdCruiseCriterion = criterionId.Value,
                                IdShip = ShipId,
                                Creator = CurrentUser,
                                CreationDate = Now,
                                Editor = CurrentUser,
                                ModificationDate = Now
                            });
                        }
                    }

                    _shoreEntities.SaveChanges();
                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Criteria, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Criterion Id : " + criterionId.ToString());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Criteria, LogManager.LogAction.Add, HttpContext.Current.User.Identity.Name, "Add Criterion Id : " + criterionId.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region Edit
        /// <summary>
        /// Modifie un critére
        /// </summary>
        /// <param name="model">Critére à modifier</param>
        public void Edit(EditCriteriaViewModel model)
        {
            string CurrentUser = HttpContext.Current.User.Identity.Name;
            DateTime Now = DateTime.Now;

            using (DbContextTransaction dbContextTransaction = _shoreEntities.Database.BeginTransaction())
            {
                try
                {
                    // Modification du criterion
                    CruiseCriterion criterion = _shoreEntities.CruiseCriterion.Find(model.Id);
                    criterion.IdSurvey = model.Survey;
                    criterion.IdCruiseType = model.CruiseType ?? 0;
                    criterion.Order = model.Order;
                    criterion.Cruise = model.Cruise;
                    criterion.Length = (model.Length.HasValue && model.Length == 0) ? null : model.Length;
                    criterion.Editor = CurrentUser;
                    criterion.ModificationDate = Now;
                    criterion.Activity = model.Activity;

                    // Suppression et insertion des destinations associées
                    criterion.CruiseCriterionDestination.Clear();
                    if (model.Destinations != null)
                    {
                        foreach (int DestinationId in model.Destinations)
                        {
                            criterion.CruiseCriterionDestination.Add(new CruiseCriterionDestination()
                            {
                                IdCruiseCriterion = criterion.Id,
                                IdDestination = DestinationId,
                                Creator = CurrentUser,
                                CreationDate = Now,
                                Editor = CurrentUser,
                                ModificationDate = Now
                            });
                        }
                    }

                    // Suppression et insertion des bateaux associés
                    criterion.CruiseCriterionShip.Clear();
                    if (model.Ships != null)
                    {
                        foreach (int ShipId in model.Ships)
                        {
                            criterion.CruiseCriterionShip.Add(new CruiseCriterionShip()
                            {
                                IdCruiseCriterion = criterion.Id,
                                IdShip = ShipId,
                                Creator = CurrentUser,
                                CreationDate = Now,
                                Editor = CurrentUser,
                                ModificationDate = Now
                            });
                        }
                    }

                    _shoreEntities.SaveChanges();
                    dbContextTransaction.Commit();
                    LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Criteria, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Criterion Id : " + model.Id.ToString());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Criteria, LogManager.LogAction.Edit, HttpContext.Current.User.Identity.Name, "Edit Criterion Id : " + model.Id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                    throw;
                }
            }
        }
        #endregion

        #region IsInUse
        /// <summary> Contrôle avant suppression
        /// Définit si des booking empéche la suppression du critère de croisière
        /// </summary>
        /// <param name="id">Id du critére</param>
        /// <returns>Vrai si le critére est supprimable, faux sinon</returns>
        public bool IsInUse(int id)
        {
            CruiseCriterion criterion = _shoreEntities.CruiseCriterion.Find(id);
            List<int> listLanguageId = (from l in criterion.Survey.Language select l.Id).Distinct().ToList();

            int nbUse = (from bcp in _shoreEntities.BookingCruisePassenger
                         where DbFunctions.AddDays(bcp.Cruise.SailingDate, bcp.Cruise.SailingLengthDays) >= DateTime.Now &&
                         listLanguageId.Contains(bcp.Booking.IdSurveyLanguage ?? 0)
                         select bcp.Booking.Id).Distinct().Count();
            return nbUse > 0;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Suppression d'un critére
        /// </summary>
        /// <param name="id">Identifiant du critére</param>
        public void Delete(int id)
        {
            try
            {
                CruiseCriterion cruiseCriterion = _shoreEntities.CruiseCriterion.Find(id);
                cruiseCriterion.CruiseCriterionDestination.Clear();
                cruiseCriterion.CruiseCriterionShip.Clear();
                _shoreEntities.CruiseCriterion.Remove(cruiseCriterion);
                _shoreEntities.SaveChanges();
                LogManager.InsertLog(LogManager.LogLevel.Info, LogManager.LogType.Criteria, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Criterion Id : " + id.ToString());
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogManager.LogLevel.Error, LogManager.LogType.Criteria, LogManager.LogAction.Delete, HttpContext.Current.User.Identity.Name, "Delete Criterion Id : " + id.ToString() + " (" + string.Concat(ex.Message, ex.InnerException != null ? " || " + ex.InnerException.Message : null) + ")");
                throw;
            }
        }
        #endregion
    }
    #endregion
}