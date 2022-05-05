using Ponant.Medical.Board.Data;
using Ponant.Medical.Board.Helpers;
using Ponant.Medical.Board.Interfaces;
using Ponant.Medical.Board.Model;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Board.View;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Ponant.Medical.Board.ViewModel
{
    /// <summary>
    /// Gestion des interactions sur le processus d'authentification
    /// </summary>
    public class AuthenticationViewModel : BaseViewModel, IViewModel
    {
        #region Properties
        private readonly IAuthenticationService authenticationService;
        private readonly DelegateCommand loginCommand;
        private readonly DelegateCommand logoutCommand;
        private readonly DelegateCommand resetCommand;
        private readonly DelegateCommand changeCommand;
        private string username;
        private string status;
        private UserControl viewToShow;

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username"); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }

        public bool IsAuthenticated
        {
            get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }

        public string Name
        {
            get { return Thread.CurrentPrincipal.Identity.Name; }
        }

        public Action CloseAction { get; set; }

        public UserControl ViewToShow
        {
            get
            {
                if (viewToShow == null)
                {
                    viewToShow = new LoginView();
                }
                return viewToShow;
            }
            set
            {
                if (value == viewToShow)
                    return;

                viewToShow = value;
                OnPropertyChanged("ViewToShow");
                OnPropertyChanged("Title");
                OnPropertyChanged("Height");
            }
        }

        public string Title
        {
            get
            {
                return ViewToShow is LoginView ? "Connection" : "Password update";
            }
        }
        #endregion

        #region Constructors
        public AuthenticationViewModel()
        {
            authenticationService = new AuthenticationService();
            loginCommand = new DelegateCommand(Login, CanLogin);
            logoutCommand = new DelegateCommand(Logout, CanLogout);
            resetCommand = new DelegateCommand(Reset, CanReset);
            changeCommand = new DelegateCommand(Change, CanChange);
        }
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get { return loginCommand; } }
        public DelegateCommand LogoutCommand { get { return logoutCommand; } }
        public DelegateCommand ResetCommand { get { return resetCommand; } }
        public DelegateCommand ChangeCommand { get { return changeCommand; } }
        #endregion

        #region Login
        /// <summary>
        /// Connexion à l'application
        /// </summary>
        /// <param name="parameter"></param>
        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            User user = null;

            //Validate credentials through the authentication service
            try
            {
                user = authenticationService.AuthenticateUser(Username, clearTextPassword);

                //Get the current principal object
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

                //Authenticate the user
                customPrincipal.Identity = new CustomIdentity(user.UserName, new string[] { "Board" });

                //Update UI
                OnPropertyChanged("AuthenticatedUser");
                OnPropertyChanged("IsAuthenticated");
                loginCommand.RaiseCanExecuteChanged();
                logoutCommand.RaiseCanExecuteChanged();
                Username = string.Empty; //reset
                passwordBox.Password = string.Empty; //reset
                Status = string.Empty;

                Application.Current.Properties["UserName"] = user.UserName;

                Application.Current.Properties["IdShip"] = user.IdShip;


                int nbDaysToExpire = user.ExpirationDate.Subtract(DateTime.Today).Days;

                if (!user.PasswordChange && (nbDaysToExpire <= 0 || (nbDaysToExpire <= 30 && MessageBox.Show(string.Format("Your password will expire in {0} days, do you want to change it now?", nbDaysToExpire), "Password expiration", MessageBoxButton.YesNo) == MessageBoxResult.Yes)))
                {
                    ViewToShow = new ChangePasswordView();
                }
                else if (user.PasswordChange || DateTime.Now > user.ExpirationDate)
                {
                    ViewToShow = new ChangePasswordView();
                }
                else
                {
                    ShowMainWindow(clearTextPassword);
                }
            }
            catch (Exception exception)
            {
                Status = exception.Message;
            }
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }
        #endregion

        #region Logout
        /// <summary>
        /// Déconnexion
        /// </summary>
        /// <param name="parameter"></param>
        private void Logout(object parameter)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                OnPropertyChanged("AuthenticatedUser");
                OnPropertyChanged("IsAuthenticated");
                loginCommand.RaiseCanExecuteChanged();
                logoutCommand.RaiseCanExecuteChanged();
                Status = string.Empty;
            }
        }

        private bool CanLogout(object parameter)
        {
            return IsAuthenticated;
        }
        #endregion

        #region Reset
        /// <summary>
        /// Réinitialisation du mot de passe
        /// </summary>
        /// <param name="parameter"></param>
        private void Reset(object parameter)
        {
            if (MessageBox.Show("Are you sure to reset your password?", "Password reset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                authenticationService.ResetPassword();
                MessageBox.Show("The passwords have been reset", "Password reset");
            }
        }

        private bool CanReset(object parameter)
        {
            return !IsAuthenticated;
        }
        #endregion

        #region Change
        /// <summary>
        /// Modification du mot de passe
        /// </summary>
        /// <param name="parameter"></param>
        private async void Change(object parameter)
        {
            List<object> parameters = parameter as List<object>;
            PasswordBox OldPassword = parameters[0] as PasswordBox;
            PasswordBox NewPassword = parameters[1] as PasswordBox;
            PasswordBox PasswordConfirmation = parameters[2] as PasswordBox;

            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$");
            if (!regex.IsMatch(NewPassword.Password))
            {
                Status = "The password must be composed of at least one digit and one capital letter on 8 characters.";
                return;
            }

            if (!NewPassword.Password.Equals(PasswordConfirmation.Password))
            {
                Status = "New passwords do not match.";
                return;
            }

            try
            {
                await authenticationService.ChangePassword(Thread.CurrentPrincipal.Identity.Name, OldPassword.Password, NewPassword.Password);
                ShowMainWindow(NewPassword.Password);
                ShoreService.Instance.UpdateHeaders();
            }
            catch (Exception exception)
            {
                Status = exception.Message;
            }
        }

        private bool CanChange(object parameter)
        {
            return IsAuthenticated;
        }
        #endregion

        #region ShowMainWindow
        /// <summary>
        /// Affiche la fenêtre principale de l'application
        /// </summary>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        private void ShowMainWindow(string password)
        {
            Application.Current.Properties["Password"] = password;
            
            MainWindow mainWindow = new MainWindow(new MainViewModel());
            var r = mainWindow;
            App.Current.MainWindow = mainWindow;
            mainWindow.Show();
            CloseAction();
        }
        #endregion
    }
}
