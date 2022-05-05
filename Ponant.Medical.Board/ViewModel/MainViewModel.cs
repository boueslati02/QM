using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.Model;
using Ponant.Medical.Board.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des interactions sur la fenêtre principale
    /// </summary>
    public class MainViewModel : BaseViewModel, IMainViewModel, IViewModel
    {
        #region Attributes

        /// <summary>
        /// Commande executée lors du chargement de la fenêtre principale
        /// </summary>
        private readonly DelegateCommand contentRenderedCommand;

        /// <summary>
        /// Commande exécutée lors du clic sur le bouton d'aide
        /// </summary>
        private readonly DelegateCommand helpCommand;

        /// <summary>
        /// nombre de tâches de fond en cours
        /// </summary>
        private int backgroundOperationCount;

        /// <summary>
        /// Page active dans la vue des questionnaires à traiter
        /// </summary>
        private IBasePageViewModel activePage;

        /// <summary>
        /// Message de date limite des QMs à traiter
        /// </summary>
        private string qmMessageToTreat;

        /// <summary>
        /// Permet de déclencher un évènement lorsque les tâches en arrière-plan sont terminées
        /// </summary>
        ManualResetEvent noBackgroundOperationEvent = new ManualResetEvent(false);

        #endregion

        #region Accessors

        /// <summary>
        /// 
        /// </summary>
        public Action CloseAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand ContentRenderedCommand
        {
            get
            {
                return contentRenderedCommand;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand HelpCommand
        {
            get
            {
                return helpCommand;
            }
        }

        /// <summary>
        /// Retourne/Positionne le nom du docteur
        /// </summary>
        public string DoctorName
        {
            get
            {
                return Application.Current.Properties["UserName"].ToString();
            }
        }

        /// <summary>
        /// Retourne la numero de version de l'application
        /// </summary>
        public string VersionNumber
        {
            get
            {
                Version versionNumber = Assembly.GetExecutingAssembly().GetName().Version;
                string strVersionNumber = "Version " + versionNumber.Major.ToString() +
                        (versionNumber.Minor != 0 ? "." + versionNumber.Minor.ToString() : "") +
                        (versionNumber.Build != 0 ? "." + versionNumber.Build.ToString() : "") +
                        (versionNumber.Revision != 0 ? "." + versionNumber.Revision.ToString() : "");

                return strVersionNumber;
            }
        }

        /// <summary>
        /// Retourne/Positionne l'état d'activité d'une tâche de fond. 
        /// Permet l'affichage de l'anneau de progression et d'interdire la fermeture de l'application si au moins une tâche est en cours.
        /// </summary>
        public bool IsBackgroundOperationActive
        {
            get
            {
                return backgroundOperationCount > 0;
            }
            set
            {
                if (value)
                {
                    backgroundOperationCount++;
                    noBackgroundOperationEvent.Reset();
                }
                else
                {
                    backgroundOperationCount--;
                    if (backgroundOperationCount == 0)
                    {
                        noBackgroundOperationEvent.Set();
                    }
                }
                OnPropertyChanged("IsBackgroundOperationActive");
            }
        }

        /// <summary>
        /// Retourne la liste des croisières
        /// </summary>
        public List<CruiseItemViewModel> CruiseItems
        {
            get
            {
                if (Application.Current.Properties[AppSettings.IdShip] == null)
                {
                    throw new ArgumentException("No boat is assigned to the current user. Please contact the administrator");
                }

                string usersIdShip = Application.Current.Properties[AppSettings.IdShip].ToString();
                using (BoardEntities db = new BoardEntities())
                {
                    return db.Cruise
                        .Where(c => DbFunctions.TruncateTime(c.SailingDate) >= DbFunctions.AddDays(DateTime.Today, -c.SailingLengthDays) && c.IdShip.ToString().Equals(usersIdShip))
                        .OrderBy(c => c.SailingDate)
                        .Take(5)
                        .Select(c => new CruiseItemViewModel
                    {
                        Entity = c
                    }).ToList();
                }
            }
        }

        /// <summary>
        /// Retourne/Positionne la page active dans la vue des questionnaires à traiter
        /// </summary>
        public IBasePageViewModel ActivePage
        {
            get
            {
                if (activePage == null)
                {
                    activePage = new SurveyToDoPage1ViewModel(this);
                }
                return activePage;
            }
            set
            {
                activePage = value;
                OnPropertyChanged("ActivePage");
            }
        }

        /// <summary>
        /// Retourne le message de date limite des QMs à traiter
        /// </summary>
        public string QmMessageToTreat
        {
            get
            {
                return qmMessageToTreat;
            }
            set
            {
                qmMessageToTreat = value;
                OnPropertyChanged("QmMessageToTreat");
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public MainViewModel()
        {
            contentRenderedCommand = new DelegateCommand(OnContentRendered);
            helpCommand = new DelegateCommand(OnHelpClick);
        }
        #endregion

        #region Public methods

        #region ConfirmClosing
        /// <summary>
        /// Affiche un message de demande de confirmation lorsque l'on ferme l'application 
        /// et qu'il y a des tâches de fond en cours
        /// </summary>
        /// <returns>La Task d'attente de fin des tâches de fond</returns>
        public async Task<bool> ConfirmClosing()
        {
            if (IsBackgroundOperationActive)
            {
                Task<bool> work = new Task<bool>(() =>
                {
                    MessageBoxResult mbr = MessageBox.Show("There are some background operations in progress" +
                        "\n\nPress NO button to cancel the closing" +
                        "\n\nPress YES button to automatically close the application when background operations finish.",
                        "Are you sure to want close the application", MessageBoxButton.YesNo);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        noBackgroundOperationEvent.WaitOne();   // attente fin des tâches de fonds
                        return true;
                    }
                    return false;
                });

                work.Start();
                return await work;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region LaunchBackgroundTasks
        /// <summary>
        /// Lance les tâches de synchronisation en arrière-plan
        /// </summary>
        /// <param name="mainViewModel">Instance du view model</param>
        /// <returns></returns>
        public static async Task LaunchBackgroundTasks(MainViewModel mainViewModel)
        {
            try
            {
                NetworkService.Instance.MainViewModel = mainViewModel;

                mainViewModel.IsBackgroundOperationActive = true;

                // MAJ des Lov 
                await LovModel.UpdateLov();

                // Libération des anciennes croisères extraites à bord et à terre
                await CruiseModel.FreeOldCruises();

                // Nettoyage des croisières
                await Task.Run(() => CruiseModel.DeleteCruises());

                // Libération des croisières extraites sur la terre
                await CruiseModel.CheckExtractCruise();

                // Envoi des avis
                await PassengerModel.SendAdvices();

                // Détachement des documents
                await DocumentModel.Unlink();

                // MAJ des croisières
                await CruiseModel.DownloadCruises();

                // MAJ des passagers
                await PassengerModel.DownloadPassengers(null, null, false, true);                
            }
            finally
            {
                mainViewModel.IsBackgroundOperationActive = false;
            }
        }
        #endregion

        #endregion

        #region Private

        #region OnContentRendered
        /// <summary>
        /// Gère les tâches de fond à effectuer dès que la fenêtre principale de l'application est affichée
        /// </summary>
        /// <param name="parameter">NU</param>
        private async void OnContentRendered(object parameter)
        {
            // Lance les tâches d'arrière plan
            await LaunchBackgroundTasks(this);
            GetQmMessageToTreat();

            // Réactualise la page si elle est active
            if (ActivePage is SurveyToDoPage1ViewModel)
            {
                SurveyToDoPage1ViewModel page = ActivePage as SurveyToDoPage1ViewModel;
                page.OnPropertyChanged("AvailableCruiseItems");
            }

            if (ActivePage is SurveyToDoPage2ViewModel)
            {
                SurveyToDoPage2ViewModel page = ActivePage as SurveyToDoPage2ViewModel;
                page.RefreshIndicators();
                page.OnPropertyChanged("SurveyBoardCount");
                page.OnPropertyChanged("SurveyProcessedCount");
                page.OnPropertyChanged("SurveySentCount");
            }

            OnPropertyChanged("CruiseItems");
        }
        #endregion

        #region OnHelpClick
        /// <summary>
        /// Affichage du fichier d'aide de l'application
        /// </summary>
        /// <param name="parameter"></param>
        private void OnHelpClick(object parameter)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), AppSettings.DocumentFolder, "Board manual.pdf");

            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path));
            }
        }
        #endregion

        #region GetQmMessageToTreat
        /// <summary>
        /// Retourne le message de date limite des QMs à traiter
        /// </summary>
        private void GetQmMessageToTreat()
        {
            using (BoardEntities db = new BoardEntities())
            {
                string usersIdShip = Application.Current.Properties[AppSettings.IdShip].ToString();
                Cruise cruise = db.Cruise.OrderBy(c => c.SailingDate).FirstOrDefault(c => c.SurveyNumberAvailable > 0 && c.IdShipAssigned.ToString() == usersIdShip);

                QmMessageToTreat =  cruise == null ? "" : $"You have { cruise.SurveyNumberAvailable } surveys to complete before { cruise.Deadline.Value.ToShortDateString() }.";
            }
        }
        #endregion

        #endregion
    }
}