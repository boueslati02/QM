using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Model;
using System;
using System.Windows;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Classe de gestion des intéractions pour chaque document du passager (ligne du datagrid)
    /// </summary>
    public class DocumentItemViewModel
    {
        #region Attributes

        /// <summary>
        /// Statut du document
        /// </summary>
        private string status;

        /// <summary>
        /// Affichage du statut
        /// </summary>
        private bool hasStatus;

        /// <summary>
        /// Commande d'ouverture d'un document
        /// </summary>
        private readonly DelegateCommand openCommand;

        /// <summary>
        /// Commande de détachement d'un document
        /// </summary>
        private readonly DelegateCommand detachCommand;

        #endregion

        #region Accessors
        /// <summary>
        /// Retourne la commande d'ouverture d'un document
        /// </summary>
        public DelegateCommand OpenCommand { get { return openCommand; } }

        /// <summary>
        /// Retourne la commande de détachement d'un document
        /// </summary>
        public DelegateCommand DetachCommand { get { return detachCommand; } }

        /// <summary>
        /// Retourne/Positionne l'identifiant du document
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom du document
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Retourne/Positionne le nom du fichier
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Retourne/Positionne la date du document
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Retourne/Positionne le statut de visualisation du document
        /// </summary>
        public string Status
        {
            get { return status; }
            set { status = value; hasStatus = true; }
        }

        /// <summary>
        /// Affichage du statut
        /// </summary>
        public bool HasStatus
        {
            get { return hasStatus; }
            set { hasStatus = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public DocumentItemViewModel()
        {
            openCommand = new DelegateCommand(Open);
            detachCommand = new DelegateCommand(Detach);
        }
        #endregion

        #region Private Methods

        #region Detach
        /// <summary>
        /// Ouvre le document sélectionné
        /// </summary>
        /// <param name="parameter">Tableau d'objets comportant ce viewmodel et le viewmodel parent</param>
        private static async void Detach(object parameter)
        {
            object[] parameters = parameter as object[];
            DocumentItemViewModel documentItem = parameters[0] as DocumentItemViewModel;
            DocumentsViewModel parent = parameters[1] as DocumentsViewModel;

            try
            {
                if (MessageBox.Show("Are you sure to want unlink the document to the passenger?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    await DocumentModel.Unlink(documentItem.Id);
                    parent.OnPropertyChanged("DocumentItems");  // actualise la vue
                }
            }
            catch (Exception exception)
            {
                Logger.Log("DocumentItemViewModel", "Detach", exception);
#if DEV || INTEGRATION
                throw new Exception("DocumentItemViewModel.Detach", exception);
#endif
            }
        }
        #endregion

        #region Open
        /// <summary>
        /// Ouvre le document sélectionné
        /// </summary>
        /// <param name="parameter">Tableau d'objets comportant ce viewmodel et le viewmodel parent</param>
        private static void Open(object parameter)
        {
            object[] parameters = parameter as object[];
            DocumentItemViewModel documentItem = parameters[0] as DocumentItemViewModel;
            DocumentsViewModel parent = parameters[1] as DocumentsViewModel;

            try
            {
                DocumentModel.Open(documentItem);
                parent.OnPropertyChanged("DocumentItems");  // actualise la vue
            }
            catch (Exception exception)
            {
                Logger.Log("DocumentItemViewModel", "Open", exception);
#if DEV || INTEGRATION
                throw new Exception("DocumentItemViewModel.Open", exception);
#endif
            }
        }
        #endregion

        #endregion
    }
}
