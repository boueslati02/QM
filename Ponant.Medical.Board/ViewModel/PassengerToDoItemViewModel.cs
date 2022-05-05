using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.View;
using System;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions pour chaque passager à traiter (ligne du datagrid)
    /// </summary>
    public class PassengerToDoItemViewModel : PassengerDoneItemViewModel, IPassengerDocumentView
    {
        #region Attributes

        /// <summary>
        /// Commande de visualisation des documents
        /// </summary>
        private readonly DelegateCommand viewDocumentsCommand;

        /// <summary>
        /// Commande de visualisation des documents
        /// </summary>
        private readonly DelegateCommand giveAdviceCommand;

        #endregion

        #region Accessors

        /// <summary>
        /// Retourne la commande de visualisation des documents
        /// </summary>
        public DelegateCommand ViewDocumentsCommand { get { return viewDocumentsCommand; } }

        /// <summary>
        /// Retourne la commande pour donner un avis
        /// </summary>
        public DelegateCommand GiveAdviceCommand { get { return giveAdviceCommand; } }

        /// <summary>
        /// Retourne/Positionne l'Id du passager
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de la croisière
        /// </summary>
        public int IdCruise { get; set; }

        /// <summary>
        /// Date de reception du Qm
        /// </summary>
        public DateTime? QmReceiptDate { get; set; }

        /// <summary>
        /// Display Qm Alert
        /// </summary>
        public bool DisplayQmAlert  { get; set; }

        /// <summary>
        /// Adresse electronique du passager
        /// </summary>
        public string Email { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public PassengerToDoItemViewModel()
        {
            viewDocumentsCommand = new DelegateCommand(ViewDocuments);
            giveAdviceCommand = new DelegateCommand(GiveAdvice);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Execute la commande de visualisation des documents
        /// </summary>
        /// <param name="parameter">Viewmodel du passager sélectionné</param>
        private static void ViewDocuments(object parameter)
        {
            try
            {
                PassengerToDoItemViewModel passenger = parameter as PassengerToDoItemViewModel;
                DocumentsView view = new DocumentsView(new DocumentsViewModel(passenger, true));
                view.Owner = App.Current.MainWindow;
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerToDoItemViewModel", "ViewDocuments", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerToDoItemViewModel.ViewDocuments", exception);
#endif
            }
        }

        /// <summary>
        /// Execute la commande pour donner un avis
        /// </summary>
        /// <param name="parameter">Tableau de deux objets : ce viewmodel du passager sélectionné et le viewmodel de la page contenant ce viewmodel</param>
        private static void GiveAdvice(object parameter)
        {
            try
            {
                object[] parameters = parameter as object[];
                PassengerToDoItemViewModel passenger = parameters[0] as PassengerToDoItemViewModel;
                SurveyToDoPage2ViewModel page = parameters[1] as SurveyToDoPage2ViewModel;
                if (page != null)
                {
                    UpdateAdviceViewModel vm = new UpdateAdviceViewModel(passenger, page);
                    UpdateAdviceView view = new UpdateAdviceView(vm);
                    view.Owner = App.Current.MainWindow;
                    if(view.ShowDialog() == true)
                    {
                        page.RefreshIndicators();
                        page.OnPropertyChanged("PassengerToDoItems");
                        page.OnPropertyChanged("PassengerDoneItems");
                        page.OnPropertyChanged("PassengerToDoCountString");
                        page.OnPropertyChanged("PassengerDoneCountString");
                        page.OnPropertyChanged("SurveyBoardCount");
                        page.OnPropertyChanged("SurveyProcessedCount");
                        page.OnPropertyChanged("SurveySentCount");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log("PassengerItemViewModel", "GiveAdvice", exception);
#if DEV || INTEGRATION
                throw new Exception("PassengerItemViewModel.GiveAdvice", exception);
#endif
            }
        }
        #endregion
    }
}