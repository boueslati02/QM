using MahApps.Metro.Controls;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.ViewModel;
using System;

namespace Ponant.Medical.Board.View
{
    /// <summary>
    /// Logique d'interaction pour AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : MetroWindow, IView
    {
        #region Constructor
        public AuthenticationView(AuthenticationViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            if (ViewModel.CloseAction == null)
            {
                ViewModel.CloseAction = new Action(() => this.Close());
            }
        }
        #endregion

        #region IView Members
        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
            set { DataContext = value; }
        }
        #endregion
    }
}
