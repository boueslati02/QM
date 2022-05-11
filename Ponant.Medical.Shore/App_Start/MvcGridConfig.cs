namespace Ponant.Medical.Shore
{
    using MVCGrid.Models;
    using MVCGrid.Web;
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.Shore.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #region MvcGridConfig
    /// <summary>
    /// Définition des grilles
    /// </summary>
    public class MvcGridConfig
    {
        public static void RegisterGrids()
        {
            #region  Parameters

            IShoreEntities shoreEntities = new ShoreEntities();
            ApplicationDbContext applicationDbContext = new ApplicationDbContext();

            GridDefaults gridDefaults = new GridDefaults()
            {
                Sorting = true,
                NoResultsMessage = "No results were found"
            };

            ColumnDefaults colDefaults = new ColumnDefaults()
            {
                EnableSorting = true
            };

            #endregion

            #region Grille des utilisateurs
            MVCGridDefinitionTable.Add("Users",
            new MVCGridBuilder<vUser>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {

                    cols.Add("AgencyName")
                        .WithValueExpression(p => p.AgencyName)
                        .WithHeaderText("Agency Name");
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName);
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName);
                    cols.Add("Login")
                        .WithValueExpression(p => p.Login);
                    cols.Add("Email")
                        .WithValueExpression(p => p.Email);
                    cols.Add("Enabled")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.Enabled ? "enabled" : "disabled")
                        .WithValueTemplate("<span class='circle {Value}' title='{Value}'></span>")
                        .WithHeaderText("Shore access");
                    cols.Add("Role")
                        .WithValueExpression(p => p.Role);
                    cols.Add("Ship")
                        .WithValueExpression(p => p.Ship);
                    cols.Add("LogoName")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.LogoName.GetUserLogosLink(p.Id))
                        .WithHeaderText("Logo")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(false);
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id)
                        .WithValueTemplate("<a class='action glyphicon glyphicon-repeat' href='/User/Reset/{Value}' title='Reset the password' onclick='return confirm(\"Are you sure you want to reset the password of this user ?\")'></a>" +
                        "<a idelement='{Value}' class='action glyphicon glyphicon-pencil' href='#' title='Edit' onclick='getModalView(this, \"/User/_Edit/{Value}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/User/Delete/{Value}' title='Delete' onclick='return confirm(\"Are you sure you want to delete this user ?\")'></a>");
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("userFilter", "agencyFilter", "roleFilter", "enabledFilter")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                return GridUtilityExtensions.DataUsers(context);
            })
            );
            #endregion

            #region Grille des utilisateurs de l'agence
            MVCGridDefinitionTable.Add("AgencyUsers",
            new MVCGridBuilder<vUser>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("AgencyName")
                        .WithValueExpression(p => p.AgencyName)
                        .WithHeaderText("Agency Name");
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName);
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName);
                    cols.Add("Login")
                        .WithValueExpression(p => p.Login);
                    cols.Add("Email")
                        .WithValueExpression(p => p.Email);
                    cols.Add("Enabled")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.Enabled ? "enabled" : "disabled")
                        .WithValueTemplate("<span class='circle {Value}' title='{Value}'></span>")
                        .WithHeaderText("Shore access");
                    cols.Add("Role")
                        .WithValueExpression(p => p.Role);
                    cols.Add("Ship")
                        .WithValueExpression(p => p.Ship);
                    cols.Add("LogoName")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.LogoName.GetUserLogosLink(p.Id))
                        .WithHeaderText("Logo");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id)
                        .WithValueTemplate("<a class='action glyphicon glyphicon-repeat' href='/User/Reset/{Value}' title='Reset the password' onclick='return confirm(\"Are you sure you want to reset the password of this user ?\")'></a>" +
                        "<a idelement='{Value}' class='action glyphicon glyphicon-pencil' href='#' title='Edit' onclick='getModalView(this, \"/User/_Edit/{Value}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/User/Delete/{Value}' title='Delete' onclick='return confirm(\"Are you sure you want to delete this user ?\")'></a>");
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("userFilter", "agencyFilter", "roleFilter", "enabledFilter")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                return GridUtilityExtensions.DataUsers(context);
            })
            );
            #endregion

            #region Grille des critères
            MVCGridDefinitionTable.Add("Criteria",
            new MVCGridBuilder<vCriteria>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Order")
                        .WithValueExpression(c => c.Order.ToString())
                        .WithHeaderText("N°");
                    cols.Add("CruiseType")
                        .WithValueExpression(c => c.CruiseType)
                        .WithHeaderText("Type of Cruise");
                    cols.Add("Destination")
                        .WithValueExpression(c => c.Destination)
                        .WithHeaderText("Destination");
                    cols.Add("Ship")
                        .WithValueExpression(c => c.Ship)
                        .WithHeaderText("Ship");
                    cols.Add("Length")
                        .WithValueExpression(c => c.Length.HasValue ? c.Length.Value.ToString() : string.Empty)
                         .WithHeaderText("Length (> days)");
                    cols.Add("Cruise")
                        .WithValueExpression(c => c.Cruise)
                        .WithHeaderText("Cruise");
                    cols.Add("Activity")
                        .WithValueExpression(c => c.Activity)
                        .WithHeaderText("Activity");
                    cols.Add("Survey")
                        .WithValueExpression(c => c.Survey)
                        .WithHeaderText("Survey");
                    cols.Add("ModificationDate")
                        .WithValueExpression(c => c.ModificationDate.ToShortDateString())
                        .WithHeaderText("Date");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(c => "col-action col-action-icon")
                        .WithValueExpression(c => c.Id.GetCriteriaActionLink(shoreEntities));
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithFiltering(false)
            .WithSorting(true, "Order", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;
                IQueryable<vCriteria> criteria = db.vCriteria;

                if (options != null && options.SortColumnData != null)
                {
                    criteria = criteria.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vCriteria>()
                {
                    Items = criteria.ToList(),
                    TotalRecords = criteria.Count()
                };
            })
            );
            #endregion

            #region Grille des questionnaires
            MVCGridDefinitionTable.Add("Survey",
            new MVCGridBuilder<vSurvey>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Name")
                        .WithValueExpression(s => s.Name)
                        .WithHeaderText("Name");
                    cols.Add("Languages")
                        .WithValueExpression(s => s.Languages)
                        .WithHeaderText("Languages");
                    cols.Add("NbIndividualSurveys")
                        .WithValueExpression(s => s.NbIndividualSurveys.ToString())
                        .WithHeaderText("Nb individual surveys");
                    cols.Add("NbIndividualSurveyMail")
                        .WithValueExpression(s => s.NbIndividualSurveyMail.ToString())
                        .WithHeaderText("Request individual mails");
                    cols.Add("NbGroupSurveys")
                       .WithValueExpression(s => s.NbGroupSurveys.ToString())
                       .WithHeaderText("Nb Group Surveys");
                    cols.Add("NbGroupSurveyMail")
                        .WithValueExpression(s => s.NbGroupSurveyMail.ToString())
                        .WithHeaderText("Request group mails");
                    cols.Add("MedicalAdvice")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(s => "text-center")
                        .WithValueExpression(s => s.MedicalAdvice ? "enabled" : "disabled")
                        .WithValueTemplate("<span class='circle {Value}' title='{Value}'></span>")
                        .WithHeaderText("Medical advice");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(s => "col-action col-action-icon")
                        .WithValueExpression(s => s.Id.ToString())
                        .WithValueTemplate("<a class='action glyphicon glyphicon-pencil' href='/Survey/Edit/{Value}' title='Edit this survey'></a><a class='action glyphicon glyphicon-trash' href='/Survey/Delete/{Value}' title='Delete this survey' onclick='return confirm(\"Are you sure you want to delete this survey ?\")'></a>");
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithFiltering(false)
            .WithSorting(true, "Name", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;
                IQueryable<vSurvey> surveys = db.vSurvey;

                if (options != null && options.SortColumnData != null)
                {
                    surveys = surveys.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vSurvey>()
                {
                    Items = surveys.ToList(),
                    TotalRecords = surveys.Count()
                };
            })
            );
            #endregion

            #region Grille des langages
            MVCGridDefinitionTable.Add("Language",
            new MVCGridBuilder<Language>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("LovLanguage.Name")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.LovLanguage.Name)
                        .WithHeaderText("Language");
                    cols.Add("IndividualSurveyFileName")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.IndividualSurveyFileName.GetLanguageFilesLink(1, l.Id))
                        .WithHeaderText("Individual survey");
                    cols.Add("IndividualSurveyMail")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.IndividualSurveyMail.GetLanguageFilesLink(3, l.Id))
                        .WithHeaderText("Request individual mail");
                    cols.Add("EmailFormat")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.EmailFormat)
                        .WithHeaderText("Email sent to passengers");
                    cols.Add("GroupSurveyFileName")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.GroupSurveyFileName.GetLanguageFilesLink(2, l.Id))
                        .WithHeaderText("Group survey");
                    cols.Add("GroupSurveyMail")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.GroupSurveyMail.GetLanguageFilesLink(4, l.Id))
                        .WithHeaderText("Request group mail");
                    cols.Add("GroupAutomaticResponse")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(l => l.GroupAutomaticResponse.GetLanguageFilesLink(8, l.Id))
                        .WithHeaderText("Automatic group response");
                    cols.Add("IsDefault")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(s => "text-center")
                        .WithValueExpression(l => l.IsDefault ? "enabled" : "disabled")
                        .WithValueTemplate("<span class='circle {Value}' title='{Value}'></span>")
                        .WithHeaderText("Default");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(l => "col-action col-action-icon")
                        .WithValueExpression(l => l.Id.ToString())
                        .WithValueTemplate("<a idelement='{Value}' class='action glyphicon glyphicon-pencil' href='#' title='Edit this language definition' onclick='getModalView(this, \"/Language/_Edit/{Value}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/Language/Delete/{Value}' title='Delete this language definition' onclick='return confirm(\"Are you sure you want to delete this language definition ?\")'></a>");
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithFiltering(false)
            .WithSorting(true, "LovLanguage.Name", SortDirection.Asc)
            .WithPageParameterNames("IdSurvey")
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                int.TryParse(options.GetPageParameterString("IdSurvey"), out int idSurvey);
                var ttt = db.Language.Where(l => l.Id == 45).ToList();
                IQueryable<Language> language = (from lang in db.Language where lang.IdSurvey.Equals(idSurvey) select lang);

                if (options?.SortColumnData != null)
                {
                    language = language.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }
                var test = new QueryResult<Language>()
                {
                    Items = language.ToList(),
                    TotalRecords = language.Count()
                };
                return test;
            })
            );
            #endregion

            #region Grille des croisières
            MVCGridDefinitionTable.Add("Cruise",
            new MVCGridBuilder<vCruiseShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Code")
                        .WithValueExpression(c => c.Code)
                        .WithHeaderText("Cruise");
                    cols.Add("TypeCruise")
                        .WithValueExpression(c => c.TypeCruise)
                        .WithHeaderText("Type of cruise");
                    cols.Add("DateDeparture")
                        .WithValueExpression(c => c.DateDeparture.ToShortDateString())
                        .WithHeaderText("Date of departure");
                    cols.Add("Destination")
                        .WithValueExpression(c => c.Destination)
                        .WithHeaderText("Destination");
                    cols.Add("Ship")
                        .WithValueExpression(c => c.Ship)
                        .WithHeaderText("Ship");
                    cols.Add("Agency")
                        .WithValueExpression(c => c.Agency)
                        .WithHeaderText("Agency");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(c => "col-action col-action-icon")
                        .WithValueExpression(c => c.Id.GetCruiseActionsLink(c.IsExtract));
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("cruiseCodeFilter", "cruisePassengerFilter", "cruiseShipFilter", "cruiseDateDepartureFilter", "cruiseTypeFilter", "cruiseDestinationFilter", "cruiseAgencyFilter")
            .WithSorting(true, "Code", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities dbShore = DependencyResolver.Current.GetService<ShoreEntities>();

                ApplicationDbContext dbAuth = DependencyResolver.Current.GetService<ApplicationDbContext>();
                UserClass _userClass = new UserClass(dbAuth);
                string userId = _userClass.GetUserId(HttpContext.Current.User.Identity.Name);

                QueryOptions options = context.QueryOptions;

                string cruiseCodeFilter = options.GetAdditionalQueryOptionString("cruiseCodeFilter");
                string cruisePassengerFilter = options.GetAdditionalQueryOptionString("cruisePassengerFilter");
                string cruiseShipFilter = options.GetAdditionalQueryOptionString("cruiseShipFilter");
                string strCruiseDateDepartureFilter = options.GetAdditionalQueryOptionString("cruiseDateDepartureFilter");
                string cruiseTypeFilter = options.GetAdditionalQueryOptionString("cruiseTypeFilter");
                string cruiseDestinationFilter = options.GetAdditionalQueryOptionString("cruiseDestinationFilter");
                string cruiseAgencyFilter = options.GetAdditionalQueryOptionString("cruiseAgencyFilter");

                DateTime? dateDeparture = null;
                if (!string.IsNullOrEmpty(strCruiseDateDepartureFilter))
                {
                    if (DateTime.TryParse(strCruiseDateDepartureFilter, out DateTime tmpDateDeparture))
                    {
                        dateDeparture = tmpDateDeparture;
                    }
                }

                IQueryable<vCruiseShore> cruises = (from cruise in dbShore.vCruiseShore
                                                    where
                                                    (cruise.Code.Contains(cruiseCodeFilter) || string.IsNullOrEmpty(cruiseCodeFilter)) &&
                                                    (cruise.IdShip.ToString() == cruiseShipFilter || cruiseShipFilter.Equals("0") || string.IsNullOrEmpty(cruiseShipFilter)) &&
                                                    (DbFunctions.TruncateTime(cruise.DateDeparture) == DbFunctions.TruncateTime(dateDeparture) || !dateDeparture.HasValue) &&
                                                    (cruise.IdTypeCruise.ToString() == cruiseTypeFilter || cruiseTypeFilter.Equals("0") || string.IsNullOrEmpty(cruiseTypeFilter)) &&
                                                    (cruise.IdDestination.ToString() == cruiseDestinationFilter || cruiseDestinationFilter.Equals("0") || string.IsNullOrEmpty(cruiseDestinationFilter)) &&
                                                    (cruise.Agency.Contains(cruiseAgencyFilter) || string.IsNullOrEmpty(cruiseAgencyFilter))
                                                    select cruise);

                if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
                {
                    int agencyId = _userClass.GetAgencyId(userId);

                    cruises = (from cruise in dbShore.vCruiseShore
                               from aar in dbShore.AgencyAccessRight
                               where
                               cruise.Code.Equals(aar.CruiseCode) && aar.IdAgency.Equals(agencyId) &&
                               (cruise.Code.Contains(cruiseCodeFilter) || string.IsNullOrEmpty(cruiseCodeFilter)) &&
                               (cruise.IdShip.ToString() == cruiseShipFilter || cruiseShipFilter.Equals("0") || string.IsNullOrEmpty(cruiseShipFilter)) &&
                               (DbFunctions.TruncateTime(cruise.DateDeparture) == DbFunctions.TruncateTime(dateDeparture) || !dateDeparture.HasValue) &&
                               (cruise.IdTypeCruise.ToString() == cruiseTypeFilter || cruiseTypeFilter.Equals("0") || string.IsNullOrEmpty(cruiseTypeFilter)) &&
                               (cruise.IdDestination.ToString() == cruiseDestinationFilter || cruiseDestinationFilter.Equals("0") || string.IsNullOrEmpty(cruiseDestinationFilter)) &&
                               (cruise.Agency.Contains(cruiseAgencyFilter) || string.IsNullOrEmpty(cruiseAgencyFilter))
                               select cruise).Distinct();
                }

                if (!string.IsNullOrEmpty(cruisePassengerFilter)) // Traitement séparer pour le filtre passager en vue d'éviter un produit cartesien
                {
                    List<int> IdCruiseList = (from p in dbShore.Passenger
                                              join bcp in dbShore.BookingCruisePassenger on p.Id equals bcp.IdPassenger
                                              where
                                              ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(cruisePassengerFilter) ? (p.LastName + " " + p.FirstName) : cruisePassengerFilter)
                                              || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(cruisePassengerFilter) ? (p.FirstName + " " + p.LastName) : cruisePassengerFilter)
                                              || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(cruisePassengerFilter) ? (p.UsualName + " " + p.FirstName) : cruisePassengerFilter)
                                              || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(cruisePassengerFilter) ? (p.FirstName + " " + p.UsualName) : cruisePassengerFilter))
                                              select bcp.IdCruise).Distinct().ToList();

                    cruises = (from cruise in cruises where IdCruiseList.Contains(cruise.Id) select cruise);
                }

                if (options?.SortColumnData != null)
                {
                    cruises = cruises.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                IEnumerable<vCruiseShore> cruisesView = cruises;

                if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
                {
                    string agencyName = _userClass.GetAgencyName(userId);
                    cruisesView = cruises.AsEnumerable().Select(c => { c.Agency = agencyName; return c; });
                }

                return new QueryResult<vCruiseShore>()
                {
                    Items = cruisesView.ToList(),
                    TotalRecords = cruises.Count()
                };
            })
            );
            #endregion

            #region Grille des passagers individuel
            MVCGridDefinitionTable.Add("IndividualPassenger",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Check")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-select")
                        .WithValueExpression(p => p.Id.GetPassengerRelaunch(p.IdStatus, "Indiv"))
                        .WithHeaderText("");
                    cols.Add("BookingNumber")
                        .WithValueExpression(p => p.BookingNumber.ToString())
                        .WithHeaderText("N° booking");
                    cols.Add("Civility")
                       .WithValueExpression(p => p.Civility)
                       .WithHeaderText("Title");
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName)
                        .WithHeaderText("LastName");
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName");
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName)
                        .WithHeaderText("FirstName");
                    cols.Add("Email")
                        .WithValueExpression(p => p.Email)
                        .WithHeaderText("Email");
                    cols.Add("EmailUpdated")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.EmailUpdated ? "glyphicon glyphicon-ok" : "hide")
                        .WithValueTemplate("<span class='{Value}'></span>")
                        .WithHeaderText("Email updated");
                    cols.Add("ValidEmail")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.ValidEmail.GetValueOrDefault(false) ? "glyphicon glyphicon-ok" : "glyphicon glyphicon-question-sign")
                        .WithValueTemplate("<span class='{Value}'></span>")
                        .WithHeaderText("Valid email");
                    cols.Add("QMStatus")
                        .WithValueExpression(p => p.QMStatus)
                        .WithHeaderText("QM status");
                    cols.Add("SendingDate")
                        .WithValueExpression(p => p.SendingDate.HasValue ? p.SendingDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Sending date");
                    cols.Add("ReceiptDate")
                        .WithValueExpression(p => p.ReceiptDate.HasValue ? p.ReceiptDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Receipt date");
                    cols.Add("NbSent")
                       .WithValueExpression(p => p.NbSent.ToString())
                       .WithHeaderText("Nb sent");
                    cols.Add("BookingAgency")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.BookingAgency.GetPassengerAgency())
                        .WithHeaderText("Agency");
                    cols.Add("AutoAttachment")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.AutoAttachment.GetAutoAttachment())
                        .WithHeaderText("Auto Attachment");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id.GetPassengerActionsLink(p.IdCruise, p.IdBooking, p.IdAdvice, p.IsEnabled));
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("nameFilter", "qmStatusFilter", "officeFilter")
            .WithPageParameterNames("IdCruise")
            .WithSorting(true, "BookingNumber", SortDirection.Asc)
            .WithRowCssClassExpression(p => (p.IdAdvice == Constants.ADVICE_FAVORABLE_OPINION || p.IdAdvice == Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS) ? "favorableAdvice" : (p.IdAdvice == Constants.ADVICE_UNFAVORABLE_OPINION) ? "unfavorableAdvice" : (p.IdAdvice == Constants.ADVICE_WAITING_FOR_CLARIFICATION) ? "waitingAdvice" : "")
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string IdCruise = options.GetPageParameterString("IdCruise");
                string nameFilter = options.GetAdditionalQueryOptionString("nameFilter");
                string qmStatusFilter = options.GetAdditionalQueryOptionString("qmStatusFilter");
                string officeFilter = options.GetAdditionalQueryOptionString("officeFilter");

                IQueryable<vPassengerShore> passengers = (from p in db.vPassengerShore
                                                          where (!p.IsGroup) &&
                             (p.IdCruise.ToString() == IdCruise) &&
                             ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.LastName + " " + p.FirstName) : nameFilter)
                             || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.LastName) : nameFilter)
                             || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.UsualName + " " + p.FirstName) : nameFilter)
                             || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.UsualName) : nameFilter)) &&
                             (p.IdStatus.ToString() == qmStatusFilter || qmStatusFilter.Equals("0") || string.IsNullOrEmpty(qmStatusFilter)) &&
                             (officeFilter.Contains(p.BookingIdOffice.ToString()) || officeFilter.Equals("0") || string.IsNullOrEmpty(officeFilter))
                                                          select p);

                if (options?.SortColumnData != null)
                {
                    passengers = passengers.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passengers.ToList(),
                    TotalRecords = passengers.Count()
                };
            })
            );
            #endregion

            #region Grille des passagers groupe
            MVCGridDefinitionTable.Add("GroupPassenger",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Check")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-select")
                        .WithValueExpression(p => p.Id.GetPassengerRelaunch(p.IdStatus, "Group"))
                        .WithHeaderText("")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(false);
                    cols.Add("Civility")
                        .WithValueExpression(p => p.Civility)
                        .WithHeaderText("Title")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName)
                        .WithHeaderText("LastName")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName)
                        .WithHeaderText("FirstName")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("Email")
                        .WithValueExpression(p => p.Email)
                        .WithHeaderText("Email")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(false);
                    cols.Add("EmailUpdated")
                       .WithHtmlEncoding(false)
                       .WithCellCssClassExpression(p => "text-center")
                       .WithValueExpression(p => p.EmailUpdated ? "glyphicon glyphicon-ok" : "hide")
                       .WithValueTemplate("<span class='{Value}'></span>")
                       .WithHeaderText("Email updated")
                       .WithAllowChangeVisibility(true)
                       .WithVisibility(false);
                    cols.Add("ValidEmail")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.ValidEmail.GetValueOrDefault(false) ? "glyphicon glyphicon-ok" : "glyphicon glyphicon-question-sign")
                        .WithValueTemplate("<span class='{Value}'></span>")
                        .WithHeaderText("Valid email")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("QMStatus")
                        .WithValueExpression(p => p.QMStatus)
                        .WithHeaderText("QM status")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("SendingDate")
                        .WithValueExpression(p => p.SendingDate.HasValue ? p.SendingDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Sending date")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("ReceiptDate")
                        .WithValueExpression(p => p.ReceiptDate.HasValue ? p.ReceiptDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Receipt date")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("NbSent")
                       .WithValueExpression(p => p.NbSent.ToString())
                       .WithHeaderText("Nb sent")
                       .WithAllowChangeVisibility(true)
                       .WithVisibility(true);
                    cols.Add("AutoAttachment")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.AutoAttachment.GetAutoAttachment())
                        .WithHeaderText("Auto Attachment")
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(true);
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id.GetPassengerActionsLink(p.IdCruise, p.IdBooking, p.IdAdvice, p.IsEnabled))
                        .WithAllowChangeVisibility(true)
                        .WithVisibility(false);
                })
            .WithQueryOnPageLoad(false)
            .WithPreloadData(false)
            .WithPaging(true, int.MaxValue)
            .WithRowCssClassExpression(p => (p.IdAdvice == Constants.ADVICE_FAVORABLE_OPINION || p.IdAdvice == Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS) ? "favorableAdvice" : (p.IdAdvice == Constants.ADVICE_UNFAVORABLE_OPINION) ? "unfavorableAdvice" : (p.IdAdvice == Constants.ADVICE_WAITING_FOR_CLARIFICATION) ? "waitingAdvice" : "")
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("IdBookingFilter", "nameFilter", "qmStatusFilter")
            .WithPageParameterNames("IdCruise")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string IdCruise = options.GetPageParameterString("IdCruise");
                string IdBookingFilter = options.GetAdditionalQueryOptionString("IdBookingFilter");
                string nameFilter = options.GetAdditionalQueryOptionString("nameFilter");
                string qmStatusFilter = options.GetAdditionalQueryOptionString("qmStatusFilter");

                IQueryable<vPassengerShore> passengers = (from p in db.vPassengerShore
                                                          where (p.IsGroup) &&
                                                          p.LastName != "TBA" &&
                                                         (p.IdCruise.ToString() == IdCruise) &&
                                                         (p.IdBooking.ToString() == IdBookingFilter) &&
                                                         ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.LastName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.LastName) : nameFilter)
                                                         || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.UsualName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.UsualName) : nameFilter)) &&
                                                            (p.IdStatus.ToString() == qmStatusFilter || qmStatusFilter.Equals("0") || string.IsNullOrEmpty(qmStatusFilter))
                                                          select p);

                if (options?.SortColumnData != null)
                {
                    passengers = passengers.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passengers.ToList(),
                    TotalRecords = passengers.Count()
                };
            })
            );
            #endregion

            #region Grille des passagers à traiter
            MVCGridDefinitionTable.Add("TodoPassenger",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName)
                        .WithHeaderText("LastName");
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName");
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName)
                        .WithHeaderText("FirstName");
                    cols.Add("PhoneNumber")
                        .WithValueExpression(p => p.PhoneNumber)
                        .WithHeaderText("Phone number");
                    cols.Add("Email")
                        .WithValueExpression(p => p.Email)
                        .WithHeaderText("Email");
                    cols.Add("Advice")
                        .WithValueExpression(p => p.Advice)
                        .WithHeaderText("Advice");
                    cols.Add("Review")
                        .WithValueExpression(p => p.Review)
                        .WithHeaderText("Comments");
                    cols.Add("QMStatus")
                        .WithValueExpression(p => p.QMStatus)
                        .WithHeaderText("QM status");
                    cols.Add("Reason")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.Id.GetInformations(shoreEntities, applicationDbContext))
                        .WithHeaderText("Reason");
                    cols.Add("Doctor")
                       .WithValueExpression(p => p.Doctor)
                       .WithHeaderText("Doctor");
                    cols.Add("TreatmentDate")
                        .WithValueExpression(p => p.TreatmentDate.HasValue ? p.TreatmentDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Date");
                    cols.Add("AutoAttachment")
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "text-center")
                        .WithValueExpression(p => p.AutoAttachment.GetAutoAttachment())
                        .WithHeaderText("Auto Attachment");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id.GetMedicalActionsLink(p.IdCruise, p.IdAdvice, p.IsEnabled, true));
                })
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithAdditionalQueryOptionNames("nameFilter", "adviceFilter", "doctorFilter")
            .WithPageParameterNames("cruiseId")
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string cruiseId = options.GetPageParameterString("cruiseId");
                string nameFilter = options.GetAdditionalQueryOptionString("nameFilter");
                string adviceFilter = options.GetAdditionalQueryOptionString("adviceFilter");
                string doctorFilter = options.GetAdditionalQueryOptionString("doctorFilter");

                IQueryable<vPassengerShore> passengers = (from p in db.vPassengerShore
                                                          where (p.IdCruise.ToString() == cruiseId) &&
                                                          (p.IdStatus.Equals(Constants.SHORE_STATUS_QM_RECEIVED) || p.IdStatus.Equals(Constants.SHORE_STATUS_QM_NEW_DOCUMENTS)) &&
                                                         ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.LastName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.LastName) : nameFilter)
                                                         || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.UsualName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.UsualName) : nameFilter)) &&
                                                         (p.IdAdvice.ToString() == adviceFilter || adviceFilter.Equals("0") || string.IsNullOrEmpty(adviceFilter)) &&
                                                         (p.DoctorId == doctorFilter || string.IsNullOrEmpty(doctorFilter))
                                                          select p);

                if (options?.SortColumnData != null)
                {
                    passengers = passengers.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passengers.ToList(),
                    TotalRecords = passengers.Count()
                };
            })
            );
            #endregion

            #region Grille des passagers traités
            MVCGridDefinitionTable.Add("DonePassenger",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName)
                        .WithHeaderText("LastName");
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName");
                    cols.Add("FirstName")
                        .WithValueExpression(p => p.FirstName)
                        .WithHeaderText("FirstName");
                    cols.Add("PhoneNumber")
                       .WithValueExpression(p => p.PhoneNumber)
                       .WithHeaderText("Phone number");
                    cols.Add("Email")
                       .WithValueExpression(p => p.Email)
                       .WithHeaderText("Email");
                    cols.Add("Advice")
                      .WithValueExpression(p => p.Advice)
                      .WithHeaderText("Advice");
                    cols.Add("Review")
                      .WithValueExpression(p => p.Review)
                      .WithHeaderText("Comments");
                    cols.Add("QMStatus")
                        .WithValueExpression(p => p.QMStatus)
                        .WithHeaderText("QM status");
                    cols.Add("Reason")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.Id.GetInformations(shoreEntities, applicationDbContext))
                        .WithHeaderText("Reason");
                    cols.Add("Doctor")
                       .WithValueExpression(p => p.Doctor)
                       .WithHeaderText("Doctor");
                    cols.Add("TreatmentDate")
                        .WithValueExpression(p => p.TreatmentDate.HasValue ? p.TreatmentDate.Value.ToShortDateString() : null)
                        .WithHeaderText("Date");
                })
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("nameFilter", "adviceFilter", "doctorFilter")
            .WithPageParameterNames("cruiseId")
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string cruiseId = options.GetPageParameterString("cruiseId");
                string nameFilter = options.GetAdditionalQueryOptionString("nameFilter");
                string adviceFilter = options.GetAdditionalQueryOptionString("adviceFilter");
                string doctorFilter = options.GetAdditionalQueryOptionString("doctorFilter");

                IQueryable<vPassengerShore> passengers = (from p in db.vPassengerShore
                                                          where (p.IdCruise.ToString() == cruiseId) && (p.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION) || p.IdAdvice.Equals(Constants.ADVICE_WAITING_FOR_CLARIFICATION) ||
                                                          p.IdAdvice.Equals(Constants.ADVICE_FAVORABLE_OPINION_WITH_RESTRICTIONS) || p.IdAdvice.Equals(Constants.ADVICE_UNFAVORABLE_OPINION)) &&
                                                         ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.LastName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.LastName) : nameFilter)
                                                         || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.UsualName + " " + p.FirstName) : nameFilter)
                                                         || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(nameFilter) ? (p.FirstName + " " + p.UsualName) : nameFilter)) &&
                                                         (p.IdAdvice.ToString() == adviceFilter || adviceFilter.Equals("0") || string.IsNullOrEmpty(adviceFilter)) &&
                                                         (p.DoctorId == doctorFilter || string.IsNullOrEmpty(doctorFilter))
                                                          select p);

                if (options?.SortColumnData != null)
                {
                    passengers = passengers.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passengers.ToList(),
                    TotalRecords = passengers.Count()
                };
            })
            );
            #endregion

            #region Grille des documents pour un passager
            MVCGridDefinitionTable.Add("DocumentOnePassenger",
            new MVCGridBuilder<Document>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Name")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(d => d.Id.GetDocumentFilesLink(d.Name, "Medical", false))
                        .WithHeaderText("Document");
                    cols.Add("ReceiptDate")
                        .WithValueExpression(d => d.ReceiptDate.ToShortDateString())
                        .WithHeaderText("Date");
                    cols.Add("LovStatus.Name")
                        .WithValueExpression(d => d.LovStatus.Name)
                        .WithHeaderText("Status")
                        .WithCellCssClassExpression(d => "tdStatus");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(d => "col-action col-action-icon")
                        .WithValueExpression(d => d.Id.GetDocumentFilesLink(d.Name, "Medical", true));
                })
            .WithFiltering(false)
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithSorting(true, "ReceiptDate", SortDirection.Dsc)
            .WithPageParameterNames("IdPassenger")
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                int.TryParse(options.GetPageParameterString("IdPassenger"), out int idPassenger);

                IQueryable<Document> document = (from doc in db.Document
                                                 where doc.IdPassenger.Equals(idPassenger) &&
                         !string.IsNullOrEmpty(doc.FileName) && !string.IsNullOrEmpty(doc.Name)
                                                 select doc);

                if (options?.SortColumnData != null)
                {
                    document = document.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<Document>()
                {
                    Items = document.ToList(),
                    TotalRecords = document.Count()
                };
            })
            );
            #endregion

            #region Grille de gestion des documents par document
            MVCGridDefinitionTable.Add("AvailableDocument",
            new MVCGridBuilder<Document>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Check")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(d => "col-action col-action-select")
                        .WithValueExpression(d => d.Id.ToString())
                        .WithHeaderText("")
                        .WithValueTemplate("<input type='checkbox' class='actionChk' idDocument='{Value}' onchange='changeCheckbox(this, \"#byDocument\")' />");
                    cols.Add("Name")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(d => d.Id.GetDocumentFilesLink(d.Name, "AvailableDocument", false))
                        .WithHeaderText("Document");
                    cols.Add("ReceiptDate")
                        .WithValueExpression(d => d.ReceiptDate.ToShortDateString())
                        .WithHeaderText("Date of receipt");
                    cols.Add("Passenger.FirstName")
                       .WithValueExpression(d => d.IdPassenger != 0 ? d.Passenger.FirstName + " " + d.Passenger.LastName : null)
                       .WithHeaderText("Passenger");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(d => "col-action col-action-icon")
                        .WithValueExpression(d => d.Id.GetAvailableDocumentActionsLink(d.Message));
                })
            .WithQueryOnPageLoad(false)
            .WithPreloadData(false)
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("documentFilter", "passengerFilter", "emailFilter", "receiptDateFilter")
            .WithSorting(true, "ReceiptDate", SortDirection.Dsc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string documentFilter = options.GetAdditionalQueryOptionString("documentFilter");
                string passengerFilter = options.GetAdditionalQueryOptionString("passengerFilter");
                string emailFilter = options.GetAdditionalQueryOptionString("emailFilter");
                string strReceiptDateFilter = options.GetAdditionalQueryOptionString("receiptDateFilter");

                DateTime? ReceiptDate = null;
                if (!string.IsNullOrEmpty(strReceiptDateFilter))
                {
                    if (DateTime.TryParse(strReceiptDateFilter, out DateTime tmpReceiptDateFilter))
                    {
                        ReceiptDate = tmpReceiptDateFilter;
                    }
                }

                IQueryable<Document> document = from doc in db.Document
                                                where
                                                (doc.Name.Contains(documentFilter) || string.IsNullOrEmpty(documentFilter)) &&
                                                ((doc.IdPassenger.ToString() == "0" && passengerFilter.ToString() == "0") || (doc.IdPassenger.ToString() != "0" && passengerFilter.ToString() == "1") || string.IsNullOrEmpty(passengerFilter)) &&
                                                (doc.Email.Contains(emailFilter) || string.IsNullOrEmpty(emailFilter)) &&
                                                (DbFunctions.TruncateTime(doc.ReceiptDate) == DbFunctions.TruncateTime(ReceiptDate) || !ReceiptDate.HasValue)
                                                select doc;

                if (options?.SortColumnData != null)
                {
                    document = document.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<Document>()
                {
                    Items = document.ToList(),
                    TotalRecords = document.Count()
                };
            })
            );
            #endregion

            #region Grille de liaison des documents à un passager
            MVCGridDefinitionTable.Add("DocumentLinkPassenger",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Radio")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(d => "col-action col-action-select")
                        .WithValueExpression(p => p.Id.ToString())
                        .WithHeaderText("")
                        .WithValueTemplate("<input type='radio' name='passengerRadio' value='{Value}' onchange='radioPassengerChange(this)'/>");
                    cols.Add("LastName")
                        .WithValueExpression(p => p.LastName)
                        .WithHeaderText("LastName");
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName");
                    cols.Add("FirstName")
                       .WithValueExpression(p => p.FirstName)
                       .WithHeaderText("FirstName");
                    cols.Add("GroupName")
                       .WithValueExpression(p => p.GroupName)
                       .WithHeaderText("Group");
                    cols.Add("CruiseCode")
                       .WithValueExpression(p => p.CruiseCode)
                       .WithHeaderText("Cruise");
                    cols.Add("BookingNumber")
                       .WithValueExpression(p => p.BookingNumber.ToString())
                       .WithHeaderText("N° booking");
                })
            .WithQueryOnPageLoad(false)
            .WithPreloadData(false)
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("linkPassengerFilter", "linkCruiseFilter", "linkBookingFilter", "linkGroupFilter")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string passengerFilter = options.GetAdditionalQueryOptionString("linkPassengerFilter");
                string cruiseFilter = options.GetAdditionalQueryOptionString("linkCruiseFilter");
                string bookingFilter = options.GetAdditionalQueryOptionString("linkBookingFilter");
                string groupFilter = options.GetAdditionalQueryOptionString("linkGroupFilter");

                // Retourne une liste vide si aucun filtre n'a été saisi
                if (string.IsNullOrWhiteSpace(passengerFilter) && string.IsNullOrWhiteSpace(cruiseFilter) && string.IsNullOrWhiteSpace(bookingFilter) && string.IsNullOrWhiteSpace(groupFilter))
                {
                    return new QueryResult<vPassengerShore>()
                    {
                        Items = new List<vPassengerShore>(),
                        TotalRecords = 0
                    };
                }

                IQueryable<vPassengerShore> passenger = from p in db.vPassengerShore
                                                        where
                                                        ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.LastName + " " + p.FirstName) : passengerFilter)
                                                        || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.FirstName + " " + p.LastName) : passengerFilter)
                                                        || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.UsualName + " " + p.FirstName) : passengerFilter)
                                                        || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.FirstName + " " + p.UsualName) : passengerFilter)) &&
                                                        (p.CruiseCode.Contains(cruiseFilter) || string.IsNullOrEmpty(cruiseFilter)) &&
                                                        (p.BookingNumber.ToString().Contains(bookingFilter) || string.IsNullOrEmpty(bookingFilter)) &&
                                                        (p.GroupName.ToString().Contains(groupFilter) || string.IsNullOrEmpty(groupFilter))
                                                        select p;

                if (options?.SortColumnData != null)
                {
                    passenger = passenger.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passenger.ToList(),
                    TotalRecords = passenger.Count()
                };
            })
            );
            #endregion

            #region Grille de gestion des documents par passager
            MVCGridDefinitionTable.Add("PassengerDocument",
            new MVCGridBuilder<vPassengerShore>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("CruiseCode")
                        .WithValueExpression(p => p.CruiseCode)
                        .WithHeaderText("Cruise");
                    cols.Add("BookingNumber")
                       .WithValueExpression(p => p.BookingNumber.ToString())
                       .WithHeaderText("N° booking");
                    cols.Add("LastName")
                       .WithValueExpression(p => p.LastName)
                       .WithHeaderText("LastName");
                    cols.Add("UsualName")
                        .WithValueExpression(p => p.UsualName)
                        .WithHeaderText("UsualName");
                    cols.Add("FirstName")
                       .WithValueExpression(p => p.FirstName)
                       .WithHeaderText("FirstName");
                    cols.Add("GroupName")
                       .WithValueExpression(p => p.GroupName)
                       .WithHeaderText("Group");
                    cols.Add("Id")
                       .WithSorting(false)
                       .WithHtmlEncoding(false)
                       .WithValueExpression(p => p.Id.GetPassengerDocuments(shoreEntities))
                       .WithHeaderText("Documents");
                    cols.Add("QMStatus")
                       .WithValueExpression(p => p.QMStatus)
                       .WithHeaderText("QM status");
                    cols.Add("Action")
                       .WithHeaderText("")
                       .WithSorting(false)
                       .WithHtmlEncoding(false)
                       .WithCellCssClassExpression(p => "col-action col-action-icon")
                       .WithValueExpression(p => p.Id.ToString())
                       .WithValueTemplate("<a idelement='{Value}' class='action glyphicon glyphicon-plus' title='Add a document to this passenger' onclick='getModalView(this, \"/PassengerDocument/_AddDocument?idPassenger={Value}\", \"frmAddDocument\", \"modalAddDocument\")'></a>");
                })
            .WithQueryOnPageLoad(false)
            .WithPreloadData(false)
            .WithPaging(true, int.MaxValue)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("passengerFilter", "cruiseFilter", "bookingFilter")
            .WithSorting(true, "LastName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string passengerFilter = options.GetAdditionalQueryOptionString("passengerFilter");
                string cruiseFilter = options.GetAdditionalQueryOptionString("cruiseFilter");
                string bookingFilter = options.GetAdditionalQueryOptionString("bookingFilter");

                // Retourne une liste vide si aucun filtre n'a été saisi
                if (string.IsNullOrWhiteSpace(passengerFilter) && string.IsNullOrWhiteSpace(cruiseFilter) && string.IsNullOrWhiteSpace(bookingFilter))
                {
                    return new QueryResult<vPassengerShore>()
                    {
                        Items = new List<vPassengerShore>(),
                        TotalRecords = 0
                    };
                }

                IQueryable<vPassengerShore> passenger = (from p in db.vPassengerShore
                                                         where
                                                         ((p.LastName + " " + p.FirstName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.LastName + " " + p.FirstName) : passengerFilter)
                                                         || (p.FirstName + " " + p.LastName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.FirstName + " " + p.LastName) : passengerFilter)
                                                         || (p.UsualName + " " + p.FirstName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.UsualName + " " + p.FirstName) : passengerFilter)
                                                         || (p.FirstName + " " + p.UsualName).Contains(string.IsNullOrEmpty(passengerFilter) ? (p.FirstName + " " + p.UsualName) : passengerFilter)
                                                         || string.IsNullOrEmpty(passengerFilter)) &&
                                                         (p.CruiseCode.Contains(cruiseFilter) || string.IsNullOrEmpty(cruiseFilter)) &&
                                                         (p.BookingNumber.ToString().Contains(bookingFilter) || string.IsNullOrEmpty(bookingFilter))
                                                         select p).Distinct();

                if (options?.SortColumnData != null)
                {
                    passenger = passenger.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<vPassengerShore>()
                {
                    Items = passenger.ToList(),
                    TotalRecords = passenger.Count()
                };
            })
            );
            #endregion

            #region Grille des droits d'accès aux agences
            MVCGridDefinitionTable.Add("AgencyAccessRight",
            new MVCGridBuilder<Data.Shore.vAgencyAccessRight>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("AgencyName")
                        .WithValueExpression(p => p.AgencyName)
                        .WithHeaderText("Agency Name");
                    cols.Add("CruiseCode")
                        .WithValueExpression(p => p.CruiseCode)
                        .WithHeaderText("Cruise Code");
                    cols.Add("GroupName")
                        .WithValueExpression(p => p.GroupName)
                        .WithHeaderText("Group ID");
                    cols.Add("BookingNumber")
                        .WithValueExpression(p => p.BookingNumber.ToString())
                        .WithHeaderText("Booking Number");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(p => p.Id.ToString())
                        .WithValueTemplate("<a idelement='{Value}' class='action glyphicon glyphicon-pencil' href='#' title='Edit' onclick='getModalView(this, \"/AgencyAccessRight/_Edit/{Value}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/AgencyAccessRight/Delete/{Value}' title='Delete' onclick='return confirm(\"Are you sure you want to delete this agency access right ?\")'></a>");
                })
            .WithPaging(false)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("Agency", "Cruise", "Group", "Booking")
            .WithSorting(true, "AgencyName", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities db = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string agency = options.GetAdditionalQueryOptionString("Agency");
                string cruise = options.GetAdditionalQueryOptionString("Cruise");
                string group = options.GetAdditionalQueryOptionString("Group");
                string booking = options.GetAdditionalQueryOptionString("Booking");

                IQueryable<Data.Shore.vAgencyAccessRight> agencyAccessRight = db.vAgencyAccessRight.Where(c =>
                    c.AgencyName.Contains(string.IsNullOrEmpty(agency) ? c.AgencyName : agency)
                    && c.CruiseCode.Contains(string.IsNullOrEmpty(cruise) ? c.CruiseCode : cruise)
                    && c.GroupName.Contains(string.IsNullOrEmpty(group) ? c.GroupName : group)
                    && c.BookingNumber.ToString().Contains(string.IsNullOrEmpty(booking) ? c.BookingNumber.ToString() : booking));

                if (options != null && options.SortColumnData != null)
                {
                    agencyAccessRight = agencyAccessRight.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<Data.Shore.vAgencyAccessRight>()
                {
                    Items = agencyAccessRight.ToList(),
                    TotalRecords = agencyAccessRight.Count()
                };
            })
            );
            #endregion

            #region Grille d'assignation des croisières
            MVCGridDefinitionTable.Add("CruiseAssignment",
            new MVCGridBuilder<Data.Shore.vAssignment>(gridDefaults, colDefaults)
                .AddColumns(cols =>
                {
                    cols.Add("Ship")
                        .WithValueExpression(a => a.Ship)
                        .WithHeaderText("ship");
                    cols.Add("Cruises")
                        .WithValueExpression(a => a.Cruises)
                        .WithHeaderText("Cruises");
                    cols.Add("Deadline")
                        .WithValueExpression(a => a.Deadline.ToShortDateString())
                        .WithHeaderText("Deadline");
                    cols.Add("Actions")
                        .WithHeaderText("")
                        .WithSorting(false)
                        .WithHtmlEncoding(false)
                        .WithCellCssClassExpression(p => "col-action col-action-icon")
                        .WithValueExpression(a => a.Id.ToString())
                        .WithValueTemplate("<a idelement='{Value}' class='action glyphicon glyphicon-pencil' href='#' title='Edit' onclick='getModalView(this, \"/CruiseAssignment/_Edit/{Value}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/CruiseAssignment/Delete/{Value}' title='Delete' onclick='return confirm(\"Are you sure you want to delete this cruise assignment ?\")'></a>");
                })
            .WithPaging(false)
            .WithClientSideLoadingCompleteFunctionName("execCompleteGridLoad")
            .WithAdditionalQueryOptionNames("CruiseShip", "CruiseCode")
            .WithSorting(true, "Ship", SortDirection.Asc)
            .WithRetrieveDataMethod((context) =>
            {
                ShoreEntities dbShore = DependencyResolver.Current.GetService<ShoreEntities>();
                QueryOptions options = context.QueryOptions;

                string CruiseShip = options.GetAdditionalQueryOptionString("CruiseShip");
                string CruiseCode = options.GetAdditionalQueryOptionString("CruiseCode");

                IQueryable<vAssignment> assignment = (from vAssignment in dbShore.vAssignment
                                                        where
                                                        (vAssignment.IdShip.ToString() == CruiseShip || CruiseShip.Equals("0") || string.IsNullOrEmpty(CruiseShip))
                                                        && (vAssignment.Cruises.Contains(CruiseCode) || string.IsNullOrEmpty(CruiseCode))
                                                        select vAssignment);

                if (options != null && options.SortColumnData != null)
                {
                    assignment = assignment.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
                }

                return new QueryResult<Data.Shore.vAssignment>()
                {
                    Items = assignment.ToList(),
                    TotalRecords = assignment.Count()
                };
            })
            );
            #endregion
        }
    }
    #endregion

    #region LinqExtensions
    /// <summary>
    /// Utilisation du orderby linq de manière plus dynamique
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>Orders the sequence by specific column and direction.</summary>
        /// <param name="query">The query.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="ascending">if set to true [ascending].</param>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortColumn, string direction)
        {
            string methodName = string.Format("OrderBy{0}",
                direction.ToLower() == "asc" ? "" : "descending");

            System.Linq.Expressions.ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(query.ElementType, "p");

            System.Linq.Expressions.MemberExpression memberAccess = null;
            foreach (var property in sortColumn.Split('.'))
                memberAccess = System.Linq.Expressions.MemberExpression.Property
                   (memberAccess ?? (parameter as System.Linq.Expressions.Expression), property);

            System.Linq.Expressions.LambdaExpression orderByLambda = System.Linq.Expressions.Expression.Lambda(memberAccess, parameter);

            System.Linq.Expressions.MethodCallExpression result = System.Linq.Expressions.Expression.Call(
                      typeof(Queryable),
                      methodName,
                      new[] { query.ElementType, memberAccess.Type },
                      query.Expression,
                      System.Linq.Expressions.Expression.Quote(orderByLambda));

            return query.Provider.CreateQuery<T>(result);
        }
    }
    #endregion

    #region UtilityExtensions
    /// <summary>
    /// Fonctions utilitaires pour l'affichage des grille
    /// </summary>
    public static class GridUtilityExtensions
    {
        #region GetLanguageFilesLink
        /// <summary>
        /// Retourne les liens pour l'accés et la suppression des fichier
        /// </summary>
        /// <param name="filename"> Nom du fichier</param>
        /// <param name="fileType">Type du fichier</param>
        /// <param name="idLanguage">id du langage</param>
        /// <returns>Lien affiché</returns>
        public static string GetLanguageFilesLink(this string filename, int fileType, int idLanguage)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string link =
                    "<div id='elipsis' title='{0}'><a href = '/Language/FileDownload?idLanguage={2}&filetype={1}'" + ((fileType == 1 || fileType == 2) ? "target='_blank'" : "") + ">{0}</a> </div> " +
                    "<a class='action glyphicon glyphicon-remove' href='/Language/FileDelete?idLanguage={2}&filetype={1}' title='Delete this document' onclick='return confirm(\"Are you sure you want to delete this document ?\")'></a>";

                return string.Format(link, filename, fileType, idLanguage);
            }

            return string.Empty;
        }
        #endregion

        #region GetUserLogosLink
        /// <summary>
        /// Retourne les liens pour l'accés et la suppression des fichier
        /// </summary>
        /// <param name="filename"> Nom du fichier</param>
        /// <param name="idUser">id de l'utilisateur</param>
        /// <returns>Lien affiché</returns>
        public static string GetUserLogosLink(this string filename, string idUser)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string link =
                    "<div id='elipsis' title='{0}'><a href='/User/LogoDownload?idUser={1}' target='_blank'>{0}</a></div>" +
                    "<a class='action glyphicon glyphicon-remove' href='/User/LogoDelete?idUser={1}' title='Delete this logo' onclick='return confirm(\"Are you sure you want to delete this logo ?\")'></a>";

                return string.Format(link, filename, idUser);
            }

            return string.Empty;
        }
        #endregion

        #region GetCriteriaActionLink
        /// <summary>
        /// Retourne les liens d'action possible sur les critéres de croisiére
        /// </summary>
        /// <param name="idCriteria">Identifiant du critére</param>
        /// <returns>Lien affiché</returns>
        public static string GetCriteriaActionLink(this int idCriteria, IShoreEntities shoreEntities)
        {
            CriteriaClass criteriaClass = new CriteriaClass(shoreEntities);
            string result = "<a idelement='{0}' class='action glyphicon glyphicon-pencil' href='#' title='Edit a criterion' onclick='getModalView(this, \"/Criteria/_Edit/{0}\", \"frmEdit\", \"modalEdit\")'></a>" +
                        "<a class='action glyphicon glyphicon-trash' href='/Criteria/Delete/{0}' title='Delete a criterion' onclick='return confirm(\"" +
                         (criteriaClass.IsInUse(idCriteria) ? "Criterion in use. " : "") + "Are you sure you want to delete this criterion ?\")'></a>";

            return string.Format(result, idCriteria);
        }
        #endregion

        #region GetCruiseActionsLink
        /// <summary>
        /// Retourne les liens d'action possible sur les croisiére
        /// </summary>
        /// <param name="cruiseId">Id de la croisiére</param>
        /// <param name="cruiseExtract">Indicateur de croisiére extraite</param>
        /// <returns>Lien affiché</returns>
        public static string GetCruiseActionsLink(this int idCruise, bool cruiseExtract)
        {
            string listUrlDest = "/Passenger?idCruise={0}";
            bool medicalUser = HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_DOCTOR) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_MEDICAL) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_MEDICAL_ADMINISTRATOR);
            if (medicalUser)
            {
                listUrlDest = "/Medical?idCruise={0}";
            }

            string result = "<a class='action glyphicon glyphicon-list' href='" + listUrlDest + "' title='View the passengers of this cruise'></a>";
            if (cruiseExtract)
            {
                bool userAutorize = HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_IT_ADMINISTRATOR) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_BOOKING_ADMINISTRATOR)
                 || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_DOCTOR) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_MEDICAL) || HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_MEDICAL_ADMINISTRATOR);

                if (userAutorize)
                {
                    result += "<a class='action glyphicon glyphicon glyphicon-lock' href='/Cruise/Unlock/{0}' title='Unlock this cruise' onclick='return confirm(\"Are you sure you want to unlock this cruise ?\")'></a>";
                }
            }
            result = string.Format(result, idCruise);
            return result;
        }
        #endregion

        #region GetPassengerAgency
        /// <summary>
        /// Retourne l'affichage pour l'agence associé au passager
        /// </summary>
        /// <param name="bookingAgency">Nom de l'agence</param>
        /// <returns>Affichage de l'agence</returns>
        public static string GetPassengerAgency(this string bookingAgency)
        {
            string result = "<span class='{0}' title='{1}'></span>";
            if (!string.IsNullOrEmpty(bookingAgency))
            {
                result = string.Format(result, "glyphicon glyphicon-ok", bookingAgency);
            }
            else
            {
                result = string.Format(result, "hide", "No agency");
            }
            return result;
        }
        #endregion

        #region GetAutoAttachment
        /// <summary>
        /// Retourne l'affichage pour auto attachment associé au passager
        /// </summary>
        /// <param name="passenger">Passager</param>
        /// <returns>Affichage de l'auto attachment</returns>
        public static string GetAutoAttachment(this bool? autoAttachment)
        {
            string result = "<span class='{0}' title='{1}'></span>";
            if (autoAttachment == null)
            {
                return "";
            }
            else
            {
                result = autoAttachment.GetValueOrDefault(false) ?
                string.Format(result, "glyphicon glyphicon-ok", "Attached automatically") :
                string.Format(result, "glyphicon glyphicon-remove", "Attached manually");
            }
            return result;
        }
        #endregion

        #region GetPassengerRelaunch
        /// <summary>
        /// Retourne l'affichage de la case à cocher d'un passager
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="qmStatus">Identifiant du status</param>
        /// <returns>Affichage de la case à cocher</returns>
        public static string GetPassengerRelaunch(this int id, int qmStatus, string IsGroup)
        {
            string result = "";
            result = "<span title='{0}'><input type='checkbox' class='relaunchChk' idPassenger='{1}' passengerStatus='{2}' {3}  onchange='changeCheckbox(this, \"#indivPassenger\")'/></span>";
            if (IsGroup == "Group")
            {
                result = "<span title='{0}'><input type='checkbox' class='relaunchChk' idPassenger='{1}' passengerStatus='{2}' {3}  onchange='changeCheckbox(this, \"#groupPassenger\")'/></span>";
            }

            if (qmStatus == Constants.SHORE_STATUS_QM_CLOSED)
            {
                result = string.Format(result, "Status of this passenger does not allow a relaunch", "", "", "disabled='disabled'");
            }
            else
            {
                result = string.Format(result, "", id, qmStatus, "");
            }
            return result;
        }
        #endregion

        #region GetPassengerActionsLink
        /// <summary>
        /// Retourne les liens d'action possible sur les passagers
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idBooking">Identifiant du booking</param>
        /// <param name="idAdvice">Identifiant de l'avis associé au passager</param>
        /// <param name="isEnabled">Indique si l'utilisateur est actif</param>
        /// <returns>Les liens d'action possible</returns>
        public static string GetPassengerActionsLink(this int id, int? idCruise, int? idBooking, int? idAdvice, bool? isEnabled)
        {
            string result = null;

            if (idCruise.HasValue)
            {
                if (isEnabled.GetValueOrDefault(false)) // Test de passager actif
                {
                    result = "<a class='action glyphicon glyphicon-ban-circle action-danger' href='/Passenger/ChangeState/{0}?idCruise={1}&idBooking={2}&enable=false' title='Disable this passenger' onclick='return confirm(\"Are you sure you want to disable this passenger ?\")'></a>";
                }
                else
                {
                    result = "<a class='action glyphicon glyphicon-ok-circle action-success' href='/Passenger/ChangeState/{0}?idCruise={1}&idBooking={2}&enable=true' title='Enable this passenger'></a>";
                }



                if (idAdvice.GetValueOrDefault(0) == 0) // Test d'avis du medecin existant
                {
                    if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY) ||
                        HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
                    {
                        result += "<a idelement='{0}' class='action glyphicon glyphicon-pencil' href='#' title='Edit this passenger' onclick='editAgencyPassenger({0},{1},{2})'></a>";
                    }
                    else
                    {
                        result += "<a idelement='{0}' class='action glyphicon glyphicon-pencil' href='#' title='Edit this passenger' onclick='editPassenger({0},{1},{2})'></a>";
                    }


                    result += "<a class='action glyphicon glyphicon-remove' href='/Passenger/Delete/{0}?idCruise={1}&idBooking={2}' title='Delete this passenger' onclick='return confirm(\"Are you sure you want to delete this passenger ?\")'></a>";
                }

                result = string.Format(result, id, idCruise, idBooking ?? 0);
            }

            return result;
        }
        #endregion

        #region GetMedicalActionsLink
        /// <summary>
        /// Retourne les liens d'action possible sur les passagers partie médical
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <param name="IdCruise">identifiant de la croisière</param>
        /// <param name="idAdvice">Identifiant de l'avis associé au passager</param>
        /// <param name="isEnabled">Indique si l'utilisateur est actif</param>
        /// <param name="isTodo">Indique s'il s'agit de la liste à traité</param>
        /// <returns>Les liens d'action possible</returns>
        public static string GetMedicalActionsLink(this int id, int? idCruise, int? idAdvice, bool? isEnabled, bool isTodo)
        {
            string result = null;
            if (idCruise.HasValue)
            {
                if (isTodo)
                {
                    result = "<a idelement='{0}' class='action glyphicon glyphicon-file' href='#' title='View the documents' onclick='getModalView(this, \"/Medical/_GetDocuments/{0}?idCruise={1}\", \"frmDocuments\", \"modalDocuments\")'></a>";
                }

                if (isTodo) // Test d'avis du medecin existant
                {
                    if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_DOCTOR)) // Test si l'utilisateur courrant est un docteur
                    {
                        result += "<a idelement='{0}' class='action glyphicon glyphicon-pencil' href='#' title='Give its opinion' onclick='getModalView(this, \"/Medical/_EditAdvice/{0}?idCruise={1}\", \"frmEditAdvice\", \"modalEditAdvice\")'></a>";
                    }
                }

                if (!string.IsNullOrWhiteSpace(result))
                {
                    result = string.Format(result, id, idCruise, isTodo ? 0 : 1);
                }
            }

            return result;
        }
        #endregion

        #region GetDocumentFilesLink
        /// <summary>
        /// Retourne les liens pour l'accés des fichiers
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <param name="filename">Nom du fichier</param>
        /// <param name="withIcon">Indique si un icone doit être affiché</param>
        /// <returns></returns>
        public static string GetDocumentFilesLink(this int id, string filename, string controllerName, bool withIcon = false)
        {
            string href = "/" + controllerName + "/FileDownload?idDocument={0}";
            string link = null;
            if (!string.IsNullOrEmpty(filename))
            {
                link = withIcon
                    ? "<a href = '" + href + "' class='action glyphicon glyphicon-folder-open linkDocument' title='Open the document' target='_blank'></a>"
                    : "<a href = '" + href + "' class='linkDocument' title='Open the document' target='_blank'>{1}</a>";
                return string.Format(link, id, filename);
            }
            return string.Empty;
        }
        #endregion

        #region GetPassengerDocuments
        /// <summary>
        /// Retourne les liens pour l'accés aux fichiers du passagers
        /// </summary>
        /// <param name="id">Identifiant du passager</param>
        /// <returns></returns>
        public static string GetPassengerDocuments(this int id, IShoreEntities shoreEntities)
        {
            string result = String.Empty;
            PassengerDocumentClass passengerDocumentClass = new PassengerDocumentClass(shoreEntities);
            List<Document> passengerDocuments = passengerDocumentClass.GetPassengerDocuments(id);
            if (passengerDocuments != null)
            {
                foreach (Document doc in passengerDocuments)
                {
                    result += string.Format("<a href='/PassengerDocument/FileDownload?idDocument={0}' target='_blank'>{1}</a> <a class='action glyphicon glyphicon-remove' href='#' title='Unlink the document from the passenger' onclick='detachDocument(this, {0})'></a><br/>", doc.Id, doc.Name);
                }

                if (result.Length > 5) // Suppression du dernier <br/>
                {
                    result = result.Substring(0, result.Length - 5);
                }
            }
            return result;
        }
        #endregion

        #region GetAvailableDocumentActionsLink
        /// <summary>
        /// Retourne les liens d'action possible sur les documents partie document management by document
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <param name="messageText">Message associè au document </param>
        /// <returns>Les liens d'action possible</returns>
        public static string GetAvailableDocumentActionsLink(this int id, string messageText)
        {
            string result = "<a idelement='{0}' class='action glyphicon glyphicon-envelope' href='#' title='View the mail' onclick='getModalView(this, \"/AvailableDocument/_GetMessage/{0}\", \"frmGetMessage\", \"modalGetMessage\")'></a>";
            if (!string.IsNullOrWhiteSpace(messageText))
            {
                return string.Format(result, id);
            }

            return string.Empty;
        }
        #endregion

        #region GetInformations
        /// <summary>
        /// Fusionne le commentaire avec les informations complémentaire saisie pour ce passager
        /// </summary>
        /// <param name="idPassenger">Identifiant du passager</param>
        /// <returns></returns>
        public static string GetInformations(this int idPassenger, IShoreEntities shoreEntities, ApplicationDbContext applicationDbContext)
        {
            MedicalClass medicalClass = new MedicalClass(shoreEntities, applicationDbContext);
            List<string> information = medicalClass.GetPassengerInformationList(idPassenger);

            if (information.Count > 0)
            {
                return "- " + string.Join("<br/> - ", information);
            }

            return null;
        }
        #endregion

        #region DataUsers
        /// <summary>
        /// Fusionne les informations des utilisateurs
        /// </summary>
        /// <param name="context">Contexte de la table</param>
        /// <returns></returns>
        public static QueryResult<vUser> DataUsers(GridContext context)
        {
            ApplicationDbContext db = DependencyResolver.Current.GetService<ApplicationDbContext>();
            UserClass _userClass = new UserClass(db);
            QueryOptions options = context.QueryOptions;

            string user = options.GetAdditionalQueryOptionString("userFilter");
            string agencyFilter = options.GetAdditionalQueryOptionString("agencyFilter");
            string agencyRestriction = null;
            string role = null;

            if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
            {
                agencyRestriction = db.vUsers.Find(_userClass.GetUserId(HttpContext.Current.User.Identity.Name)).AgencyName;
            }

            if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_BOOKING_ADMINISTRATOR))
            {
                role = Constants.ROLE_ID_BOOKING;
            }
            else if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_AGENCY_ADMINISTRATOR))
            {
                role = Constants.ROLE_ID_AGENCY;
            }
            else if (HttpContext.Current.User.IsInRole(Constants.ROLE_NAME_GROUP))
            {
                role = Constants.ROLE_ID_AGENCY_ADMINISTRATOR;
            }
            else
            {
                role = options.GetAdditionalQueryOptionString("roleFilter");
            }

            bool? enabled = null;

            if (!string.IsNullOrWhiteSpace(options.GetAdditionalQueryOptionString("enabledFilter")))
            {
                enabled = bool.Parse(options.GetAdditionalQueryOptionString("enabledFilter"));
            }

            IQueryable<vUser> users = db.vUsers.Where(u =>
                ((u.LastName + " " + u.FirstName).Contains(string.IsNullOrEmpty(user) ? (u.LastName + " " + u.FirstName) : user)
                || (u.FirstName + " " + u.LastName).Contains(string.IsNullOrEmpty(user) ? (u.FirstName + " " + u.LastName) : user))
                && u.RoleId.Equals(string.IsNullOrEmpty(role) ? u.RoleId : role)
                && u.Enabled == (enabled.HasValue ? enabled : u.Enabled)
                && u.AgencyName.Contains(string.IsNullOrEmpty(agencyFilter) ? u.AgencyName : agencyFilter)
                && u.AgencyName.Equals(string.IsNullOrEmpty(agencyRestriction) ? u.AgencyName : agencyRestriction));

            if (options.SortColumnData != null)
            {
                users = users.OrderBy(options.SortColumnData.ToString(), options.SortDirection.ToString());
            }

            return new QueryResult<vUser>()
            {
                Items = users.ToList(),
                TotalRecords = users.Count()
            };
        }
        #endregion
    }
    #endregion
}