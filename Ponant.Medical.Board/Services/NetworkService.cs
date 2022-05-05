using Ponant.Medical.Board.ViewModel;
using System.Net.NetworkInformation;

namespace Ponant.Medical.Board.Services
{
    /// <summary>
    /// Classe de gestion de la connectivité réseau
    /// </summary>
    public sealed class NetworkService
    {
        #region Properties
        /// <summary>
        /// Instance unique de cette classe
        /// </summary>
        private static NetworkService instance = new NetworkService();

        /// <summary>
        /// Retourne l'instance unique de cette classe (Singleton)
        /// </summary>
        public static NetworkService Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Instance du view model
        /// </summary>
        public MainViewModel MainViewModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructeur
        /// </summary>
        public NetworkService()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }
        #endregion

        #region Events
        /// <summary>
        /// Se produit lorsque la connexion au réseau change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                await MainViewModel.LaunchBackgroundTasks(MainViewModel);
            }
        }
        #endregion

        #region Public methods

        #region IsNetworkAvailable
        /// <summary>
        /// Permet de tester la connectivité réseau
        /// </summary>
        /// <returns>Vrai si la connexion est établi, faux sinon</returns>
        public bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        #endregion
        
        #endregion
    }
}
