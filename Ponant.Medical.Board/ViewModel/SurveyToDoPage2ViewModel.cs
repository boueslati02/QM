using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.Model;
using Ponant.Medical.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des interactions sur la croisière en cours de traitement
    /// </summary>
    public class SurveyToDoPage2ViewModel : BasePageViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande d'annulation du traitement d'une croisière
        /// </summary>
        private readonly DelegateCommand cancelCommand;

        /// <summary>
        /// Viewmodel de la croisière sélectionnée
        /// </summary>
        private readonly AvailableCruiseItemViewModel cruiseItem;

        /// <summary>
        /// Viewmodel de la page de retour
        /// </summary>
        private readonly IBasePageViewModel returnPage;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande d'annulation du traitement d'une croisière
        /// </summary>
        public DelegateCommand CancelCommand { get { return cancelCommand; } }

        /// <summary>
        /// Retourne la page de retour
        /// </summary>
        public IBasePageViewModel ReturnPage { get { return returnPage; } }

        /// <summary>
        /// Retourne la chaine du code de la croisière
        /// </summary>
        public string CruiseCodeString
        {
            get { return "State of the cruise " + cruiseItem.CruiseCode; }
        }

        /// <summary>
        /// Retourne le nombre de passagers de la croisière
        /// </summary>
        public int PassengersCount
        {
            get { return cruiseItem.Entity.PassengersNumber; }
        }

        /// <summary>
        /// Retourne le nombre de QM non disponible
        /// </summary>
        public int SurveyNotAvailable
        {
            get { return cruiseItem.Entity.SurveyNumberNotAvailable; }
        }

        /// <summary>
        /// Retourne le nombre de QM termines à bord
        /// </summary>
        public int SurveyDoneOnShore
        {
            get { return cruiseItem.Entity.SurveyNumberDoneShore; }
        }

        /// <summary>
        /// Retourne le nombre de questionnaires à bord
        /// </summary>
        public int SurveyBoardCount
        {
            get
            {
                return cruiseItem.Entity.SurveyNumberAvailable;
            }
        }

        /// <summary>
        /// Retourne le nombre de questionnares traités
        /// </summary>
        public int SurveyProcessedCount
        {
            get { return cruiseItem.Entity.SurveyNumberDoneBoard; }
        }

        /// <summary>
        /// Retourne le nombre de questionnaires envoyé sur terre
        /// </summary>
        public int SurveySentCount
        {
            get { return cruiseItem.Entity.SurveyNumberSent; }
        }

        /// <summary>
        /// Retourne la chaine du nombre de passagers à traiter
        /// </summary>
        public string PassengerToDoCountString
        {
            get
            {
                return string.Format("List of passengers to do ({0})", PassengerToDoItems != null 
                    ? PassengerToDoItems.Count 
                    : 0);
            }
        }

        /// <summary>
        /// Retourne la chaine du nombre de passagers traité
        /// </summary>
        public string PassengerDoneCountString
        {
            get
            {
                return string.Format("List of passengers already done ({0})", PassengerDoneItems != null 
                    ? PassengerDoneItems.Count 
                    : 0);
            }
        }

        /// <summary>
        /// Retourne la liste des passagers à traiter
        /// </summary>
        public List<PassengerToDoItemViewModel> PassengerToDoItems
        {
            get
            {
                using (BoardEntities db = new BoardEntities())
                {
                    List<PassengerToDoItemViewModel> result = db.Passenger.Where(p =>
                            p.Cruise.Any(c => c.Id.Equals(cruiseItem.Entity.Id)) &&
                            p.IdStatus.Equals(Constants.BOARD_STATUS_QM_TO_DO)
                        ).ToList()
                        .Select(c => new PassengerToDoItemViewModel
                        {
                            Id = c.Id,
                            IdCruise = cruiseItem.Entity.Id,
                            LastName = c.LastName,
                            UsualName = c.UsualName,
                            FirstName = c.FirstName,
                            Email = c.Email,
                            Advice = c.LovAdvice.Name,
                            Status = c.LovStatus.Name,
                            QmReceiptDate = c.QmReceiptDate,
                            DisplayQmAlert = (c.QmReceiptDate.HasValue 
                                ? ((c.QmReceiptDate.Value.AddDays(AppSettings.DelayToQmAlertDays) < DateTime.Now.Date) 
                                    ? true 
                                    : false) 
                                : false),
                            Comments = (c.Review + (string.IsNullOrEmpty(c.Review) ? (c.Information.Count > 0 ? "- " : string.Empty) : (c.Information.Count > 0 ? "\n - " : string.Empty))
                                + string.Join("\n - ", (from info in c.Information orderby info.Lov.Name ascending select info.Lov.Name).Distinct().ToList()))
                        }).ToList();
                    return result;
                }
            }
        }

        /// <summary>
        /// Retourne la liste des passagers traités
        /// </summary>
        public List<PassengerDoneItemViewModel> PassengerDoneItems
        {
            get
            {
                using (BoardEntities db = new BoardEntities())
                {
                    List<PassengerDoneItemViewModel> result = db.Passenger.Where(p => 
                            p.Cruise.Any(c => c.Id.Equals(cruiseItem.Entity.Id)) 
                            && (p.IdStatus.Equals(Constants.BOARD_STATUS_QM_DONE) || p.IdStatus.Equals(Constants.BOARD_STATUS_QM_ACQUITTED)) 
                        ).ToList()
                        .Select(c => new PassengerDoneItemViewModel
                        {
                            LastName = c.LastName,
                            UsualName = c.UsualName,
                            FirstName = c.FirstName,
                            Email = c.Email,
                            Advice = c.LovAdvice.Name,
                            Status = c.LovStatus.Name,
                            Comments = (c.Review + (string.IsNullOrEmpty(c.Review) ? (c.Information.Count > 0 ? "- " : string.Empty) : (c.Information.Count > 0 ? "\n - " : string.Empty))
                                + string.Join("\n - ", (from info in c.Information orderby info.Lov.Name ascending select info.Lov.Name).Distinct().ToList())),
                        }).ToList();
                    return result;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mainvm">Viewmodel de la vue principale</param>
        /// <param name="cruiseitem">Viewmodel de la croisière sélectionnée</param>
        /// <param name="returnpage">Page à afficher lors de l'appui sur le bouton cancel</param>
        public SurveyToDoPage2ViewModel(IMainViewModel mainvm, AvailableCruiseItemViewModel cruiseitem, IBasePageViewModel returnpage)
        {
            cancelCommand = new DelegateCommand(Cancel);
            MainViewModel = mainvm;
            cruiseItem = cruiseitem;
            returnPage = returnpage;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Rafraîchit les indicateurs de la croisière pour l'affichage à l'écran
        /// </summary>
        public void RefreshIndicators()
        {
            using (BoardEntities db = new Data.BoardEntities())
            {
                cruiseItem.Entity = db.Cruise.Single(c => c.Code.Equals(cruiseItem.CruiseCode));
            }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Affiche (retourne à) la page de appelante
        /// </summary>
        /// <param name="parameter">NU</param>
        private async void Cancel(object parameter)
        {
            if (MessageBox.Show("Are you sure you want to cancel your cruise treatment?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainViewModel.IsBackgroundOperationActive = true;

                using (BoardEntities db = new Data.BoardEntities())
                {
                    IEnumerable<Data.Cruise> cruisesList = PassengerModel.GetImminentCruise(db);
                    bool isImminentCruise = cruisesList.Select(c => c.Id).Contains(cruiseItem.Entity.Id);
                    await CruiseModel.FreeCruise(cruiseItem.Entity.Id, isImminentCruise);
                }

                // MAJ des passagers
                await CruiseModel.DownloadCruises();
                //await AvailableCruiseItemViewModel.DownloadSurveys(cruiseItem.Entity.Id, Constants.SHORE_STATUS_QM_CLOSED, Constants.BOARD_STATUS_QM_DONE, null, false);

                MainViewModel.IsBackgroundOperationActive = false;

                MainViewModel.ActivePage = returnPage;
                OnPropertyChanged("CruiseItems");

                SurveyToDoPage1ViewModel surveyToDoPage1 = returnPage as SurveyToDoPage1ViewModel;
                surveyToDoPage1.OnPropertyChanged("AvailableCruiseItems");
            }
        }
        #endregion
    }
}
