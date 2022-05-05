using Ponant.Medical.Board.Model;
using Ponant.Medical.Board.Services;
using Ponant.Medical.Board.View;
using Ponant.Medical.Board.ViewModel;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ponant.Medical.Board
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Application_DispatcherUnhandledException
        /// <summary>
        /// Affichage des messages d'erreur de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(string.Concat(e.Exception.Message, e.Exception.InnerException != null ? "\r\n" + e.Exception.InnerException.Message : null), "Application error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        /// <summary>
        /// Affichage des messages d'erreur des méthodes asynchrones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)(e.ExceptionObject);
            MessageBox.Show(string.Concat(exception.Message, exception.InnerException != null ? "\r\n" + exception.InnerException.Message : null), "Application error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region OnStartup
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Définition de la culture
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                // Init des utilisateurs
                AuthenticationService.InitUser();

                // Init des documents
                string applicationName = Application.ResourceAssembly.GetName().Name;
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, AppSettings.CurrentCruisesFolder);
                App.Current.Properties[AppSettings.CurrentCruisesFolder] = folder;
                Directory.CreateDirectory(folder);

                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, AppSettings.CruisesToDoFolder);
                App.Current.Properties[AppSettings.CruisesToDoFolder] = folder;
                Directory.CreateDirectory(folder);

                // Create a custom principal with an anonymous identity at startup
                CustomPrincipal customPrincipal = new CustomPrincipal();
                AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

                // Show the login view
                AuthenticationViewModel viewModel = new AuthenticationViewModel();
                new AuthenticationView(viewModel).Show();

                base.OnStartup(e);
            }
            catch (Exception exception)
            {
                string error = exception.Message;
                if (exception.InnerException != null)
                {
                    error += "\n\n" + exception.InnerException.Message;
                }
                MessageBox.Show(error, "Startup failure");
            }
        }
        #endregion
    }
}
