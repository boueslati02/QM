using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Model;
using Ponant.Medical.Data;
using Ponant.Medical.Data.Shore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions sur la liste des croisières récentes et imminentes du bateau
    /// </summary>
    public class AvailableCruiseItemViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande de téléchargement des questionnaires associé à une croisières
        /// </summary>
        private readonly DelegateCommand downloadSurveysCommand;

        /// <summary>
        /// Commande de traitement d'une croisière
        /// </summary>
        private readonly DelegateCommand treatCruiseCommand;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande de téléchargement des questionnaires associé à une croisière
        /// </summary>
        public DelegateCommand DownloadSurveysCommand
        {
            get
            {
                return downloadSurveysCommand;
            }
        }

        /// <summary>
        /// Retourne la commande de traitement d'une croisière
        /// </summary>
        public DelegateCommand TreatCruiseCommand
        {
            get
            {
                return treatCruiseCommand;
            }
        }

        /// <summary>
        /// Retourne/Positionne l'entité de la croisière
        /// </summary>
        public Data.Cruise Entity { get; set; }

        /// <summary>
        /// Retourne le code de la croisière
        /// </summary>
        public string CruiseCode
        {
            get
            {
                return Entity.Code;
            }
        }

        /// <summary>
        /// Retourne le nombre de passagers
        /// </summary>
        public int PassengerCount
        {
            get
            {
                return Entity.PassengersNumber;
            }
        }

        /// <summary>
        /// Retourne le nombre de questionnaires disponibles
        /// </summary>
        public int QMCount
        {
            get
            {
                return Entity.SurveyNumberAvailable;
            }
        }

        /// <summary>
        /// Retourne le nombre de questionnaires traités
        /// </summary>
        public int QMDoneCount
        {
            get
            {
                return Entity.SurveyNumberDoneBoard + Entity.SurveyNumberDoneShore;
            }
        }

        /// <summary>
        /// Retourne le nombre de questionnaires téléchargés
        /// </summary>
        public int QMDownloadedCount
        {
            get
            {
                return Entity.SurveyNumberDownloaded;
            }
        }

        /// <summary>
        /// Permet de savoir si on peut télécharger les passagers de la croisière
        /// </summary>
        public bool IsDownloadSurveysVisible
        {
            get
            {
                return !IsOneCruiseExtracted();
            }
        }

        /// <summary>
        /// Permet de savoir si on peut traiter la croisière
        /// </summary>
        public bool IsTreatCruiseVisible
        {
            get
            {
                return Entity.IsExtract;
            }
        }

        /// <summary>
        /// Permet de connaitre la date limite de la croisière
        /// </summary>
        public DateTime? Deadline
        {
            get
            {
                return Entity.Deadline;
            }
        }

        /// <summary>
        /// Permet de savoir si la croisière a été affectée au compte connecté
        /// </summary>
        public bool IsAssignedCruise
        {
            get
            {
                return Entity.IdShipAssigned.HasValue ? Entity.IdShipAssigned.Value.ToString() == Application.Current.Properties[AppSettings.IdShip].ToString() : false;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AvailableCruiseItemViewModel()
        {
            downloadSurveysCommand = new DelegateCommand(DownloadSurveys);
            treatCruiseCommand = new DelegateCommand(TreatCruise);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Télécharge les questionnaires d'une croisière
        /// </summary>
        /// <param name="idCruise">Identifiant de la croisière</param>
        /// <param name="idStatusShore">Identifiant du statut à terre des questionnaires à télécharger</param>
        /// <param name="idStatusBoard">Identifiant du statut à bord des questionnaires à télécharger</param>
        /// <param name="surveyToDoPage1ViewModel">Instance du ViewModel</param>
        public static async Task DownloadSurveys(
            int? idCruise, 
            int idStatusShore, 
            int idStatusBoard, 
            SurveyToDoPage1ViewModel surveyToDoPage1ViewModel,
            bool updateShorePassengerStatus)
        {
            try
            {
                List<PassengerCruise> passengersCruises = await PassengerModel.DownloadPassengers(idCruise, idStatusShore, false, updateShorePassengerStatus);

                if (passengersCruises != null)
                {
                    if (surveyToDoPage1ViewModel != null)
                    {
                        surveyToDoPage1ViewModel.DownloadAvailable = passengersCruises.Count;
                    }

                    foreach (PassengerCruise passengerCruise in passengersCruises)
                    {
                        await PassengerModel.DownloadPassenger(passengerCruise.IdPassenger, passengerCruise.IdCruise, idStatusBoard, updateShorePassengerStatus);

                        if (surveyToDoPage1ViewModel != null)
                        {
                            surveyToDoPage1ViewModel.DownloadReceived++;
                        }
                    }

                    if (surveyToDoPage1ViewModel != null
                        && surveyToDoPage1ViewModel.DownloadReceived == surveyToDoPage1ViewModel.DownloadAvailable)
                    {
                        surveyToDoPage1ViewModel.DownloadReceived = 0;
                        surveyToDoPage1ViewModel.DownloadAvailable = 0; // masque la progress bar
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("AvailableCruiseItemViewModel", "DownloadSurveys", exception);
#if DEV || INTEGRATION
                throw new Exception("AvailableCruiseItemViewModel.DownloadSurveys", exception);
#endif
            }
        }
        #endregion

        #region Private

        #region DownloadSurveys
        /// <summary>
        /// Télécharge les questionnaires associés à une croisière
        /// </summary>
        /// <param name="parameter">Tableau d'objets comportant ce viewmodel et le viewmodel parent</param>
        private static async void DownloadSurveys(object parameter)
        {
            object[] parameters = parameter as object[];
            AvailableCruiseItemViewModel availableCruiseItem = parameters[0] as AvailableCruiseItemViewModel;
            SurveyToDoPage1ViewModel parent = parameters[1] as SurveyToDoPage1ViewModel;

            using (BoardEntities db = new BoardEntities())
            {
                Data.Cruise cruise = db.Cruise.Find(availableCruiseItem.Entity.Id);
                cruise.IsExtract = true;
                db.SaveChanges();
            }

            await DownloadSurveys(availableCruiseItem.Entity.Id, Constants.SHORE_STATUS_QM_RECEIVED, Constants.BOARD_STATUS_QM_TO_DO, parent, false);
            parent.OnPropertyChanged("AvailableCruiseItems");
        }
        #endregion

        #region IsOneCruiseExtracted
        /// <summary>
        /// Recherche si une croisière est extraite
        /// </summary>
        /// <returns>Vrai si une croisière est extraite, faux sinon</returns>
        private static bool IsOneCruiseExtracted()
        {
            using (BoardEntities db = new Data.BoardEntities())
            {
                return db.Cruise.Any(c => c.IsExtract);
            }
        }
        #endregion

        #region TreatCruise
        /// <summary>
        /// Traite une croisière
        /// </summary>
        /// <param name="parameter">Tableau de deux objets : ce viewmodel de la croisière sélectionnée et le viewmodel de la page contenant ce viewmodel</param>
        private static void TreatCruise(object parameter)
        {
            try
            {
                object[] parameters = parameter as object[];
                AvailableCruiseItemViewModel cruise = parameters[0] as AvailableCruiseItemViewModel;
                SurveyToDoPage1ViewModel parent = parameters[1] as SurveyToDoPage1ViewModel;

                if (parent != null)
                {
                    parent.MainViewModel.ActivePage = new SurveyToDoPage2ViewModel(parent.MainViewModel, cruise, parent.MainViewModel.ActivePage);
                }
            }
            catch (Exception exception)
            {
                Logger.Log("AvailableCruiseItemViewModel", "TreatCruise", exception);
#if DEV || INTEGRATION
                throw new Exception("AvailableCruiseItemViewModel.TreatCruise", exception);
#endif
            }
        }
        #endregion

        #endregion
    }
}
