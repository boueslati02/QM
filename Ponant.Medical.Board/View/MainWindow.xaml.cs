using MahApps.Metro.Controls;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.ViewModel;
using System.Reflection;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Ponant.Medical.Board
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class MainWindow : MetroWindow, IView
    {
        #region Properties
        /// <summary>
        /// Fermeture de la fenêtre
        /// </summary>
        private bool isCloseWindow;
        #endregion

        #region IView Members
        public IViewModel ViewModel
        {
            get
            {
                return DataContext as IViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
        #endregion

        #region Constructor
        public MainWindow(MainViewModel viewmodel)
        {
            InitializeComponent();
            ViewModel = viewmodel;
        }
        #endregion

        #region MetroWindow_Closing
        /// <summary>
        /// Gestion de la fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(isCloseWindow)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.IsEnabled = false;
                await Task.Yield();

                IMainViewModel viewmodel = ViewModel as IMainViewModel;

                isCloseWindow = await viewmodel.ConfirmClosing();
                if(isCloseWindow)
                {
                    this.Close();
                }
                else
                {
                    this.IsEnabled = true;
                }
            }
        }
        #endregion
    }
}
